﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics.CodeAnalysis;

namespace DataAccess
{
    /// <summary>
    /// Analysis operations on tables, like joins, histogram, dup search, filter, etc.
    /// These handle large tables.
    /// </summary>
    public static class Analyze
    {
        /// <summary>
        /// Sort a mutable datatable in place by the given column. 
        /// </summary>
        /// <param name="dt">dat table to sort</param>
        /// <param name="columnName">column name to sort on. Throws if missing</param>
        public static void Sort(this MutableDataTable dt, string columnName)
        {
            Sort(dt, columnName, StringComparer.InvariantCultureIgnoreCase);
        }

        /// <summary>
        /// Sort a mutable datatable in place by the given column. 
        /// </summary>
        /// <param name="dt">dat table to sort</param>
        /// <param name="columnName">column name to sort on. Throws if missing</param>
        /// <param name="comparer">Comparer to use on column name</param>
        public static void Sort(this MutableDataTable dt, string columnName, IComparer<string> comparer)
        {
            if (comparer == null)
            {
                throw new ArgumentNullException("comparer");
            }
            var column = dt.GetColumn(columnName, throwOnMissing: true);

            int len = column.Values.Length;
            int[] map = new int[len];
            for (int i = 0; i < len; i++)
            {
                map[i] = i;
            }

            Array.Sort(column.Values, map, comparer);

            // Sort other columns for consistency
            foreach (var c in dt.Columns)
            {
                if (c == column)
                {
                    continue;
                }

                string[] newVals = new string[len];
                for (int i = 0; i < len; i++)
                {
                    newVals[i] = c.Values[map[i]];
                }
                c.Values = newVals;
            }
        }

        /// <summary>
        /// Given a potentially extremely large table, shred it into smaller CSV files based on the values in columnName.
        /// This can be very useful for easily building an index for a large file. 
        /// For each unique value in column, funcCreateStream is invoked with that value to get a TextWriter. The csv is written to that writer.
        /// The ordering within each small file is preserved
        /// This stream based overload is useful when you need to avoid writing to the local file system (such as with Azure storage)
        /// </summary>
        /// <param name="table">original table to shred</param>
        /// <param name="funcCreateStream">callback function to create a stream for each new table.</param>
        /// <param name="columnName">column name to use for shredding. You can use <see cref="GetColumnValueCounts"/>
        /// to see the variation in each column to determine a good column to use for shredding.
        /// </param>    
        public static void Shred(this DataTable table, string columnName, Func<string, TextWriter> funcCreateStream)
        {
            Shred(table, columnName, funcCreateStream, null);
        }

        public static void Shred(this DataTable table, string columnName, Func<string, TextWriter> funcCreateStream, Func<string,string> filter)
        {
            Dictionary<string, TextWriter> dict = new Dictionary<string, TextWriter>();

            try
            {
                foreach (Row row in table.Rows)
                {
                    TextWriter tw;
                    string val = row[columnName];
                    if (filter != null)
                    {
                        val = filter(val);
                        if (val == null)
                        {
                            continue;
                        }
                    }
                    if (!dict.TryGetValue(val, out tw))
                    {
                        // New value
                        tw = funcCreateStream(val);                        
                        dict[val] = tw;
                        CsvWriter.RawWriteLine(table.ColumnNames, tw); // header
                    }
                    CsvWriter.RawWriteLine(row.Values, tw);
                }

            }
            finally
            {
                foreach (var kv in dict)
                {
                    kv.Value.Close();
                }
            }
        }

        /// <summary>
        /// Given a potentially extremely large table, shred it into smaller CSV files based on the values in columnName.
        /// This can be very useful for easily building an index for a large file. 
        /// For each unique value in column, a CSV file is created and named string.Format(templateFilename, value).
        /// The ordering within each small file is preserved
        /// </summary>
        /// <param name="table">original table to shred</param>
        /// <param name="columnName">column name to use for shredding. You can use <see cref="GetColumnValueCounts"/>
        /// to see the variation in each column to determine a good column to use for shredding.
        /// </param>
        /// <param name="templateFilename">template specifying filename of shredded files.</param>
        public static void Shred(this DataTable table, string columnName, string templateFilename)
        {
            Func<string, TextWriter> func =
                (value) =>
                {
                    string destination = string.Format(templateFilename, value);
                    Utility.EnsureDirExistsForFile(destination);
                    TextWriter tw = new StreamWriter(destination);
                    return tw;
                };
            Shred(table, columnName, func);
        }

        public static void Shred(this DataTable table, string columnName, string templateFilename, Func<string,string> filter)
        {
            Func<string, TextWriter> func =
                (value) =>
                {
                    string destination = string.Format(templateFilename, value);
                    Utility.EnsureDirExistsForFile(destination);
                    TextWriter tw = new StreamWriter(destination);
                    return tw;
                };
            Shred(table, columnName, func, filter);
        }



        /// <summary>
        /// Apply a Where filter to a table. This can stream over large data and filter it down. 
        /// </summary>
        /// <param name="table">source table</param>
        /// <param name="fpSelector">predicate to execute on each row</param>
        /// <returns>a new table that copies out rows from from the source table</returns>
        public static DataTable Where(this DataTable table, Func<Row, bool> fpSelector)
        {
            TableWriter writer = new TableWriter(table);

            int count = 0;
            foreach(Row row in table.Rows)
            {   
                bool keep = fpSelector(row);
                if (keep)
                {
                    writer.AddRow(row);
                    count++;
                }                
            }
            return writer.CloseAndGetTable(); 
        }
                
        /// <summary>
        /// Merge 2 datatables together assuming no common join key. 
        /// This will collapse common columns, but keep all rows. 
        /// This needs to deal with columns being in different orders        
        /// </summary>
        /// <param name="tables">set of tables to merge together</param>
        /// <returns>a merged table. The rows may be in a random order.</returns>
        public static MutableDataTable Join(IEnumerable<DataTable> tables)
        {
            var dict = new Dictionary2d<string, string, string>();
            int counter = 0;
            foreach (var dt in tables)
            {
                Add(dt, dict, ref counter);
            }

            var merge = DataTable.New.From2dDictionary(dict);

            // remove extra column that Dict2d added, and reorder to more closely match dataset
            var mutable = DataTable.New.GetMutableCopy(merge);

            IEnumerable<string> columnNames = new string[0];
            foreach (var dt in tables)
            {
                columnNames = columnNames.Concat(dt.ColumnNames);
            }
            var names = columnNames.Distinct(StringComparer.OrdinalIgnoreCase);

            var x = names.ToArray();
            mutable.KeepColumns(x);

            return mutable;
        }

        static void Add(DataTable table, Dictionary2d<string, string, string> dict, ref int counter)
        {
            var names = (from name in table.ColumnNames select name.ToLowerInvariant()).ToArray();
            foreach (var row in table.Rows)
            {
                int i = 0;
                foreach (var name in names)
                {
                    var value = row.Values[i];

                    dict[counter.ToString(), name] = value;
                    i++;
                }

                counter++;
            }
        }

        // $$$ Clarify - multiple joins (inner, outer, etc)
        
        /// <summary>
        /// Performs a full outer join on two in-memory tables and returns a new table.
        /// The number of rows in the resulting table is the sum of rows from each source table.
        /// The number of columns in teh new table is the sum of columns in the the source tables minus 1 
        /// (since the join column is redundant)
        /// </summary>
        /// <param name="d1"></param>
        /// <param name="d2"></param>
        /// <param name="columnName">column name to join on. Both tables must have this column name.</param>
        /// <returns>a new table</returns>
        public static MutableDataTable Join(MutableDataTable d1, MutableDataTable d2, string columnName)
        {
            Column c1 = d1.GetColumn(columnName);
            if (c1 == null)
            {
                throw new InvalidOperationException("Missing column");
            }
            Column c2 = d2.GetColumn(columnName);
            if (c2 == null)
            {
                throw new InvalidOperationException("Missing column");
            }

            // Place d1 in first set of columns, and d2 in second set.
            int kColumn = d1.Columns.Length;
            int kTotalColumns = kColumn + d2.Columns.Length;

            // Indices into new table where join columns are.
            int joinColumn1 = Utility.GetColumnIndexFromName(d1.ColumnNames, columnName);
            int joinColumn2 = Utility.GetColumnIndexFromName(d2.ColumnNames, columnName) + kColumn;

            // $$$ could really optimize. Sort both on column and then zip.
            Dictionary<string, int> m1 = GetRowIndex(c1);
            Dictionary<string, int> m2 = GetRowIndex(c2);

            // $$$ column names may not be unique.

            //string[] headers = d1.ColumnNames.Union(d2.ColumnNames).ToArray();
            
            string[] headers = new string[kTotalColumns];
            Array.Copy(d1.ColumnNames.ToArray(), 0, headers, 0, kColumn);
            Array.Copy(d2.ColumnNames.ToArray(), 0, headers, kColumn, kTotalColumns - kColumn);

            string[] values = new string[headers.Length];

            string path = GetTempFileName();
            using (CsvWriter tw = new CsvWriter(path, headers))
            {

                foreach (var kv in m1)
                {
                    Clear(values);                    

                    string key = kv.Key; // join column
                    int r1 = kv.Value;
                    int r2;
                    if (m2.TryGetValue(key, out r2))
                    {
                        // In both.  write out
                        CopyRowIntoArray(values, kColumn, d2, r2);
    
                        m2.Remove(key);
                    }
                    else
                    {
                        // Only in M1. 
                    }

                    CopyRowIntoArray(values, 0, d1, r1);
                    values[joinColumn1] = values[joinColumn2] = key;

                    tw.WriteRow(values);
                }

                // We remove all of M1's items from m2, so M2 is just unique items now. (possibly 0).
                // Tag those onto the end.

                foreach (var kv in m2)
                {
                    int r2 = kv.Value;
                    Clear(values);
                    CopyRowIntoArray(values, kColumn, d2, r2);
                    values[joinColumn1] = values[joinColumn2] = kv.Key;

                    tw.WriteRow(values);
                }

            } // close tw

            MutableDataTable t = Reader.ReadCSV(path);
            DeleteLocalFile(path);

            // Remove duplicate columns.
            t.DeleteColumn(joinColumn2);

            return t;
        }

        static void CopyRowIntoArray(string[] values, int index, MutableDataTable d, int row)
        {
            for (int c = 0; c < d.Columns.Length; c++)
            {
                values[index] = d.Columns[c].Values[row];
                index++;
            }
        }

        static void Clear(string[] values)
        {
            for (int i = 0; i < values.Length; i++)
            {
                values[i] = string.Empty;
            }
        }

        static Dictionary<string, int> GetRowIndex(Column c)
        {
            Dictionary<string, int> d = new Dictionary<string, int>();

            for (int row = 0; row < c.Values.Length; row++)
            {
                string x = c.Values[row].ToUpperInvariant();

                // If this add fails, it means the column we're doing a join on has duplicate entries.
                d.Add(x, row); // verifies uniqueness
            }
            return d;
        }


        
        /// <summary>
        /// Return a sample that's the top N records from a table.
        /// This is useful to sample a large table and then save the sample. 
        /// </summary>
        /// <param name="table">source table</param>
        /// <param name="topN">positive value specifying number of rows to copy from from source table</param>
        /// <returns>The topN rows from the source table.</returns>
        public static MutableDataTable SampleTopN(this DataTable table, int topN)
        {
            if (topN <= 0)
            {
                throw new ArgumentOutOfRangeException("topN", "sample must be a positive integer");
            }

            TableWriter writer = new TableWriter(table);

            foreach (var row in table.Rows)
            {
                topN--;
                writer.AddRow(row);

                if (topN == 0)
                {
                    // Check topN before the enumeration to avoid pulling a wasted row from the source table
                    break;
                }
            }
                
            return DataTable.New.GetMutableCopy(writer.CloseAndGetTable()); 
        }

        private static int[] GetColumnIndexFromNames(DataTable table, string[] columnNames)
        {
            return Array.ConvertAll(columnNames, columnName => GetColumnIndexFromName(table, columnName));
        }

        // Return 0-based index of column with matching name.
        // throws an exception if not found
        private static int GetColumnIndexFromName(DataTable table, string columnName)
        {
            string[] columnNames = table.ColumnNames.ToArray();
            return Utility.GetColumnIndexFromName(columnNames, columnName);
        }


        
        /// <summary>
        /// Extract column as a histogram, sorted in descending order by frequency.        
        /// </summary>
        /// <param name="table">source table</param>
        /// <param name="columnName">column within short table</param>
        /// <returns>collection of tuples, where each tuple is a value and the count of that value within the column</returns>
        public static Tuple<string, int>[] AsHistogram(this DataTable table, string columnName)
        {
            int i = GetColumnIndexFromName(table, columnName);
            return AsHistogram(table, i);
        }

        /// <returns></returns>
        /// <summary>
        /// Extract column as a histogram, sorted in descending order by frequency.        
        /// </summary>
        /// <param name="table">source table</param>
        /// <param name="columnIdx">0-based index of column </param>
        /// <returns>collection of tuples, where each tuple is a value and the count of that value within the column</returns>
        public static Tuple<string, int>[] AsHistogram(this DataTable table, int columnIdx)
        {
            Dictionary<string, int> values = new Dictionary<string, int>();

            //string name = "unknown";
            foreach (Row row in table.Rows)
            {

                var parts = row.Values;
                if (columnIdx >= parts.Count)
                {
                    // malformed input file
                    continue;
                }
                string p = parts[columnIdx];

                int count;
                values.TryGetValue(p, out count);
                count++;
                values[p] = count;
            }

            // Get top N?

            var items = from kv in values
                        orderby kv.Value descending
                        select Tuple.Create(kv.Key, kv.Value)
                        ;

            //int N = 10;
            //return items.Take(N).ToArray();
            return items.ToArray();
        }


        /// <summary>
        /// Produces a table where each row is the number of unique values in a source column, followed by the top N occurences in that column.
        /// </summary>
        /// <param name="table">source table</param>
        /// <param name="N">number of top N occurences to include in the summary table </param>
        /// <returns>a summary table</returns>
        public static MutableDataTable GetColumnValueCounts(this DataTable table, int N)
        {
            if (N < 0)
            {
                throw new ArgumentOutOfRangeException("N");
            }

            string[] names = table.ColumnNames.ToArray();
            int count = names.Length;

            MutableDataTable dSummary = new MutableDataTable();
            Column c1 = new Column("column name", count);
            Column c2 = new Column("count", count);

            int kFixed = 2;
            Column[] cAll = new Column[kFixed + N * 2];
            cAll[0] = c1;
            cAll[1] = c2;

            for (int i = 0; i < N; i++)
            {
                cAll[i * 2 + kFixed] = new Column("Top Value " + i, count);
                cAll[i * 2 + 1 + kFixed] = new Column("Top Occurrence " + i, count);
            }
            dSummary.Columns = cAll;

            int columnId = 0;
            foreach (string name in names)
            {
                Tuple<string, int>[] hist = AsHistogram(table, columnId);

                c1.Values[columnId] = name;
                c2.Values[columnId] = hist.Length.ToString();

                for (int i = 0; i < N; i++)
                {
                    if (i >= hist.Length)
                    {
                        break;
                    }
                    cAll[i * 2 + kFixed].Values[columnId] = hist[i].Item1;
                    cAll[i * 2 + 1 + kFixed].Values[columnId] = hist[i].Item2.ToString();
                }

                columnId++;
            }

            return dSummary;
        }

        
        /// <summary>
        /// Find all rows that have dups for the given columns.
        /// This uses a multi-pass algorithm to operate on a large data file.
        /// </summary>
        /// <param name="table">original table</param>
        /// <param name="columnNames">set of columns to compare to look for duplicates</param>
        /// <returns>a table that's a subset of the original table</returns>
        public static DataTable SelectDuplicates(this DataTable table, params string[] columnNames)
        {


            int[] ci = GetColumnIndexFromNames(table, columnNames);

            // Store on hash keys first. Use hash keys because they're compact and efficient for large data sets
            // But then we do need to handle collisions. 
            HashSet<int> allKeys = new HashSet<int>();
            HashSet<int> possibleDups = new HashSet<int>();

            //
            // Take a first pass and store the hash of each row's unique Key
            //
            foreach (Row row in table.Rows)
            {
                var parts = row.Values;
                int hash = CalcHash(parts, ci);

                if (allKeys.Contains(hash))
                {
                    possibleDups.Add(hash);
                }
                else
                {
                    allKeys.Add(hash);
                }
            }
            allKeys = null; // Free up for GC

            //
            // Now take a second pass through the dups.
            //
            Dictionary<string, Row> fullMatch = new Dictionary<string, Row>();

            StringBuilder sb = new StringBuilder();

            TableWriter writer = new TableWriter(table);            
            
            foreach (Row row in table.Rows)
            {
                {
                    var parts = row.Values;
                    int hash = CalcHash(parts, ci);
                    if (!possibleDups.Contains(hash))
                    {
                        continue;
                    }

                    // Potential match                    
                    sb.Clear();
                    foreach (int i in ci)
                    {
                        sb.Append(parts[i]);
                        sb.Append(',');
                    }
                    string key = sb.ToString();

                    if (fullMatch.ContainsKey(key))
                    {
                        Row firstLine = fullMatch[key];
                        if (firstLine != null)
                        {
                            writer.AddRow(firstLine);
                            fullMatch[key] = null;
                        }

                        // Real dup!
                        writer.AddRow(row);
                    }
                    else
                    {
                        fullMatch[key] = row;
                    }
                }
            } // reader
            return writer.CloseAndGetTable();
        }

        // Helper for finding duplicates.
        private static int CalcHash(IList<string> parts, int[] ci)
        {
            int h = 0;
            foreach (int i in ci)
            {
                h += parts[i].GetHashCode();
            }
            return h;
        }

        /// <summary>
        /// For azure usage, allow hooking the function used to create temporary files
        /// </summary>
        [SuppressMessage("Microsoft.Usage", "CA2211:NonConstantFieldsShouldNotBeVisible")]
        public static Func<string> GetTempFileName = System.IO.Path.GetTempFileName;

        [SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes")]
        private static void DeleteLocalFile(string file)
        {
            try
            {
                System.IO.File.Delete(file);
            }
            catch
            {
                // Not fatal.
            }
        }
    }
}
