﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace DataAccess
{

    // This is the heart of the parsing logic. All other parsing operations should call into this. 
    // This handles all the wacky corner cases like newlines in quoted values.
    // This is internal. Use the Builder functions to access them.
    // A CSV description is here:
    // http://www.creativyst.com/Doc/Articles/CSV/CSV01.htm 
    internal class Reader
    {
        SplitState _currentState;

        List<string> _parts = new List<string>();
        StringBuilder _sb = new StringBuilder();
        bool _captureValue;

        static string Intern(string value) {
            //return string.Intern(value);
            return value;
        }
        // split the string, and then trim each part
        // items can be quoted. 
        // A, B, C
        // A, "B1, B2", C        
        // - trim whitespace 
        public static string[] split(string input, char separator) {
            return split(input, separator, true);
        }
        
        private enum SplitState
        {
            Start = 0,
            StartQuote,
            StartSeparator,
            PotentialStartSpace,
            Word,
            EscapedWord,
            PotentialEndQuote,
            PotentialEndSpace,
            UnescapedQuote,
            MissingEndQuote
        }

        public static string[] split(string input, char separator, bool trim)
        {
            Reader r = new Reader();
            r.StartRow();
            int count = 0;
            foreach (var ch in input)
            {
                r.ProcessSingleChar(ch, separator, trim);
                count++;
            }
            return r.DoneRow(trim);
        }

        public void StartRow()
        {
            rowCount++;
            _currentState = SplitState.Start;
            _parts.Clear();
            _sb.Length = 0;

            _captureValue = IncludeColumn(_parts.Count);
        }

        private bool IncludeColumn(int columnIdx)
        {
            // $$$
            return true;
        }

        public char chSeparator; // Guess the separator from the first row
        public int rowCount = -1; // before start of rist row
        public bool errorMode; // hit an error, just jump to the next newline
        public int ignore; // really bad failures. 

        const char EOFChar = unchecked((char) -1);
        public string[] ProcessEndOfFile(bool trim)
        {
            return ProcessChar(EOFChar, trim);
        }

        // Process the character. If we're at the end of a row, return the values. 
        // If we're int he middle of a row, return null.
        // accept -1 as a EOF terminator to cooperate with STream.ReadByte(). 
        public string[] ProcessChar(char ch, bool trim)
        {
            if (ch == EOFChar)
            {
                if (this.HasContent())
                {
                    ch = '\n';
                }
                else
                {
                    return null;
                }
            }
            if (ch == '\r')
            {
                if (!ShouldNewlineBeContent())
                {
                    return null;
                }
            }
            if (ch == '\n')
            {
                if (!errorMode)
                {
                    if (!ShouldNewlineBeContent())
                    {
                        var values = this.DoneRow(trim);
                        StartRow();
                        return values;
                    }
                }
                else
                {
                    errorMode = false;
                    return null;
                }
            }

            // Guess separator from contents.
            if (chSeparator == 0)
            {
                if (ch == '\t')
                {
                    chSeparator = '\t';
                }
                else if (ch == ',')
                {
                    chSeparator = ',';
                }
                else if (ch == ';')
                {
                    chSeparator = ';';
                }
                else if (ch == '|')
                {
                    chSeparator = '|';
                }
            }

            try
            {
                if (!errorMode)
                {
                    ProcessSingleChar(ch, chSeparator, trim);
                }
            }
            catch (AssertException e)
            {
                // Something really corrupt about this row. Ignore it. 
                ignore++;

                Console.WriteLine("$$$ Error at row: {0} {1}", rowCount, e.Message);
                errorMode = true;
                StartRow();
            }
            return null;
        }

        public void ProcessSingleChar(char ch, char separator, bool trim)
        {
            switch (_currentState)
            {
                case SplitState.Start:
                    if (ch == '"')
                    {
                        _currentState = SplitState.StartQuote;
                    }
                    else if (ch == separator)
                    {
                        _currentState = SplitState.StartSeparator;
                    }
                    else if (ch == ' ')
                    {
                        _currentState = SplitState.PotentialStartSpace;
                    }
                    else
                    {
                        _currentState = SplitState.Word;
                    }
                    break;
                case SplitState.StartQuote:
                    if (ch == '"')
                    {
                        _currentState = SplitState.PotentialEndQuote;
                    }
                    else
                    {
                        _currentState = SplitState.EscapedWord;
                    }
                    break;
                case SplitState.StartSeparator:
                    if (ch == '"')
                    {
                        _currentState = SplitState.StartQuote;
                    }
                    else if (ch == separator)
                    {
                        break;
                    }
                    else if (ch == ' ')
                    {
                        _currentState = SplitState.PotentialStartSpace;
                    }
                    else
                    {
                        _currentState = SplitState.Word;
                    }
                    break;
                case SplitState.PotentialStartSpace:
                    if (ch == '"')
                    {
                        _currentState = SplitState.StartQuote;
                        _sb.Length = 0;
                    }
                    else if (ch == separator)
                    {
                        _currentState = SplitState.StartSeparator;
                    }
                    else if (ch != ' ')
                    {
                        _currentState = SplitState.Word;
                    }
                    break;
                case SplitState.Word:
                    // Allow quotes in the middle of a word.
                    // a, b "b", c
                    if (ch == '"')
                    {
                        //currentState = SplitState.UnescapedQuote;
                    }
                    else if (ch == separator)
                    {
                        _currentState = SplitState.StartSeparator;
                    }
                    break;
                case SplitState.EscapedWord:
                    if (ch == '"')
                    {
                        _currentState = SplitState.PotentialEndQuote;
                    }
                    break;
                case SplitState.PotentialEndQuote:
                    if (ch == '"')
                    {
                        _currentState = SplitState.EscapedWord;
                    }
                    else if (ch == separator)
                    {
                        _currentState = SplitState.StartSeparator;
                    }
                    else if (ch == ' ')
                    {
                        _currentState = SplitState.PotentialEndSpace;
                    }
                    else
                    {
                        // Case where we had a double quote. like:
                        //  ""val"
                        //_currentState = SplitState.UnescapedQuote;

                        // Treat is as an escape. 
                        _currentState = SplitState.EscapedWord;
                    }
                    break;
                case SplitState.PotentialEndSpace:
                    if (ch == separator)
                    {
                        _currentState = SplitState.StartSeparator;
                    }
                    else if (ch != ' ')
                    {
                        _currentState = SplitState.UnescapedQuote;
                    }
                    // Anything else is a case like: "abc" d
                    // does that parse as 'abc d'?  Is it an error?
                    break;
                default:
                    break;
            }

            switch (_currentState)
            {
                case SplitState.StartSeparator:
                    {
                        PushValue(trim);
                    }
                    break;

                case SplitState.PotentialStartSpace:
                case SplitState.Word:
                case SplitState.EscapedWord:
                case SplitState.PotentialEndSpace:
                    if (_captureValue)
                    {
                        _sb.Append(ch);
                    }
                    break;

                case SplitState.UnescapedQuote:
                    throw new AssertException("unescaped double quote");

                case SplitState.MissingEndQuote:
                    throw new AssertException("missing closing quote");
            }
        }

        private void PushValue(bool trim)
        {
            string x;
            if (_captureValue)
            {

                x = _sb.ToString();
                _sb.Length = 0;
                if (trim) { x = x.Trim(); }
            }
            else
            {
                x = string.Empty;
            }
            _parts.Add(x);

            _captureValue = this.IncludeColumn(_parts.Count);
        }

        // Are we in the middle of a word? IE, should newlines count as part of the value?
        public bool ShouldNewlineBeContent()
        {
            return (_currentState == SplitState.EscapedWord) || (_currentState == SplitState.StartQuote);
        }
        public string[] DoneRow(bool trim)
        {
            // add leftovers
            PushValue(trim);
           
            return _parts.ToArray();
        }

        public bool HasContent()
        {
            if (_parts.Count > 0)
            {
                // Even if this value is empty, if there are previous values in the row, that means content.
                return true;
            }
            if (_sb == null || _sb.Length == 0)
            {
                return false;
            }
            return !string.IsNullOrWhiteSpace(_sb.ToString());
        }

        public static MutableDataTable ReadTab(string filename) {
            return Read(filename, '\t');
        }
        public static MutableDataTable ReadCSV(string filename) {
            return Read(filename, ',');
        }

         
        public static MutableDataTable ReadCSV(TextReader stream)
        {
            return Read(stream, delimiter: ',');
        }


        public static MutableDataTable Read(TextReader stream, char delimiter = '\0', string[] defaultColumns = null)
        {
            // TextReader is not seekable. Need to convert to a Stream so we can seek. 
            // We're asking for a Mutable dt anyways, so it's already expected to load it in memory. 
            string contents = stream.ReadToEnd();
            var bytes = Encoding.UTF8.GetBytes(contents);
            contents = null;
            using (MemoryStream ms = new MemoryStream(bytes))
            {
                var dtLazy = new StreamingDataTable(ms, defaultColumns, delimiter);

                var dt = Utility.ToMutable(dtLazy);
                return dt;
            }
        }

        public static MutableDataTable ReadString(string text, string newLine = "\r\n", char delimiter = '\0', string[] defaultColumns = null)
        {
            using (var tr = new StringReader(text))
            {
                return Read(tr, delimiter, defaultColumns);
            }
        }

        public static char GuessSeparateFromHeaderRow(string header)
        {
            if (header.Contains("\t"))
            {
                return '\t';
            }

            if (header.Contains(","))
            {
                return ',';
            }

            if (header.Contains(";"))
            {
                return ';';
            }

            if (header.Contains("|"))
            {
                return '|';
            }
            
            // Fallback is always comma. This implies a single column. 
            return ',';
            
        }

        // Read in a Ascii file that uses the given separate characters.
        // Like CSV. 
        // Supports quotes to escape commas
        public static MutableDataTable Read(string filename, char separator = '\0', bool fAllowMismatch = false, string[] defaultColumns = null)
        {
            var dtLazy = new FileStreamingDataTable(filename, defaultColumns, separator);

            var dt = Utility.ToMutable(dtLazy);            
            dt.Name = filename;
            return dt;
        }
    }
}