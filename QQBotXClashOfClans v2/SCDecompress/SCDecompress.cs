using SevenZip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace QQBotXClashOfClans_v2
{
    enum signatures
    {
        NONE = 0,
        LZMA = 1, // starts with 5D 00 00 04
        SC = 2, // starts with SC
        SCLZ = 3, // starts with SC and contains SCLZ
        SIG = 4, // starts with Sig:
    }
    public class SCDecompress
    {
        [DllImport("msvcrt.dll", CallingConvention = CallingConvention.Cdecl)]
        static extern int memcmp(byte[] b1, byte[] b2, long count);
        public static byte[] Decompress(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            var signature = ReadSignature(data);
            switch (signature)
            {
                case signatures.NONE:
                    return data;
                case signatures.LZMA:
                    saveHeader(path, new byte[] { 0x4c, 0x5a, 0x4d, 0x41 });
                    var uncompressedSize = INT2LE(data[5]);
                    var padded = data.Take(9).ToList();
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.AddRange(data.Skip(9));
                    return decompress(padded.ToArray());
                case signatures.SC:
                    saveHeader(path, data.Take(26).ToArray());
                    data = data.Skip(26).ToArray();
                    uncompressedSize = INT2LE(data[5]);
                    padded = data.Take(9).ToList();
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.AddRange(data.Skip(9));
                    return decompress(padded.ToArray());
                case signatures.SIG:
                    saveHeader(path, data.Take(68).ToArray());
                    data = data.Skip(68).ToArray();
                    uncompressedSize = INT2LE(data[5]);
                    padded = data.Take(9).ToList();
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.Add((byte)(uncompressedSize == -1 ? 0xFF : 0));
                    padded.AddRange(data.Skip(9));
                    return decompress(padded.ToArray());
                default:
                    Console.WriteLine($"signature { signature } is not supported");
                    return data;

            }
        }

        private static void saveHeader(string path, byte[] data)
        {
            File.WriteAllBytes(path.Remove(path.LastIndexOf('.')) + ".header", data);
        }

        public static byte[] Compress(string path)
        {
            byte[] data = File.ReadAllBytes(path);
            var signature = ReadSignature(data);
            if (signature != signatures.NONE)
            {
                Console.WriteLine("The file already compressed");
                return data;
            }
            if (File.Exists(path.Remove(path.LastIndexOf('.')) + ".header"))
            {
                //Header file is there
                var header = File.ReadAllBytes(path.Remove(path.LastIndexOf('.')) + ".header");
                if (header.Length == 4)
                {
                    //LZMA
                    var compressed = compress(data);
                    var listData = compressed.ToList();
                    listData.RemoveRange(9, 4);
                    File.Delete(path.Remove(path.LastIndexOf('.')) + ".header");
                    return listData.ToArray();
                }
                else
                {
                    //SIG or SC
                    var compressed = compress(data);
                    var listData = new List<byte>();
                    listData.AddRange(header);
                    listData.AddRange(compressed);
                    listData.RemoveRange(9, 4);
                    File.Delete(path.Remove(path.LastIndexOf('.')) + ".header");
                    return listData.ToArray();
                }
            }
            else
            {
                Console.WriteLine("No header is found in the file!");
                return data;
            }
        }

        private static signatures ReadSignature(byte[] data)
        {
            if (memcmp(data.Take(3).ToArray(), HexToByteArray("5d0000"), HexToByteArray("5d0000").Length) == 0)
            {
                return signatures.LZMA;
            }
            else if (Encoding.UTF8.GetString(data.Take(2).ToArray()).ToLower() == "sc")
            {
                if (data.Length >= 30 && Encoding.UTF8.GetString(data.Skip(26).Take(3).ToArray()).ToLower() == "sclz")
                {
                    return signatures.SCLZ;
                }
                return signatures.SC;
            }
            else if (Encoding.UTF8.GetString(data.Take(4).ToArray()).ToLower() == "sig:")
            {
                return signatures.SIG;
            }
            return signatures.NONE;
        }
        private static byte[] HexToByteArray(string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
        private static int INT2LE(byte data)
        {
            byte[] b = new byte[4];
            b[0] = data;
            b[1] = (byte)(((uint)data >> 8) & 0xFF);
            b[2] = (byte)(((uint)data >> 16) & 0xFF);
            b[3] = (byte)(((uint)data >> 24) & 0xFF);
            return BitConverter.ToInt32(b, 0);
        }

        private static byte[] compress(byte[] decompressed)
        {
            byte[] retVal = null;
            bool eos = true;
            Int32 dictionary = 1 << 16;
            Int32 posStateBits = 2;
            Int32 litContextBits = 3; // for normal files
                                      // UInt32 litContextBits = 0; // for 32-bit data
            Int32 litPosBits = 0;
            // UInt32 litPosBits = 2; // for 32-bit data
            Int32 algorithm = 2;
            Int32 numFastBytes = 64;
            string mf = "bt4";

            var propIDs = new CoderPropID[]
            {
       CoderPropID.DictionarySize,
       CoderPropID.PosStateBits,
       CoderPropID.LitContextBits,
       CoderPropID.LitPosBits,
       CoderPropID.Algorithm,
       CoderPropID.NumFastBytes,
       CoderPropID.MatchFinder,
       CoderPropID.EndMarker
            };
            var properties = new object[]
            {
       dictionary,
       posStateBits,
       litContextBits,
       litPosBits,
       algorithm,
       numFastBytes,
       mf,
       eos
            };
            SevenZip.Compression.LZMA.Encoder encoder = new SevenZip.Compression.LZMA.Encoder();
            using (Stream strmInStream = new MemoryStream(decompressed))
            {
                strmInStream.Seek(0, 0);
                using (MemoryStream strmOutStream = new MemoryStream())
                {
                    encoder.SetCoderProperties(propIDs, properties);
                    encoder.WriteCoderProperties(strmOutStream);
                    Int64 fileSize = strmInStream.Length;
                    for (int i = 0; i < 8; i++)
                    {
                        strmOutStream.WriteByte((Byte)(fileSize >> (8 * i)));
                    }
                    encoder.Code(strmInStream, strmOutStream, -1, -1, null);
                    retVal = strmOutStream.ToArray();
                }
            }
            return retVal;
        }

        private static byte[] decompress(byte[] compressed)
        {
            byte[] retVal = null;

            SevenZip.Compression.LZMA.Decoder decoder = new SevenZip.Compression.LZMA.Decoder();

            using (Stream strmInStream = new MemoryStream(compressed))
            {
                strmInStream.Seek(0, 0);

                using (MemoryStream strmOutStream = new MemoryStream())
                {
                    byte[] properties2 = new byte[5];
                    if (strmInStream.Read(properties2, 0, 5) != 5)
                        throw (new Exception("input .lzma is too short"));

                    long outSize = 0;
                    for (int i = 0; i < 8; i++)
                    {
                        int v = strmInStream.ReadByte();
                        if (v < 0)
                            throw (new Exception("Can't Read 1"));
                        outSize |= ((long)(byte)v) << (8 * i);
                    } //Next i

                    decoder.SetDecoderProperties(properties2);

                    long compressedSize = strmInStream.Length - strmInStream.Position;
                    decoder.Code(strmInStream, strmOutStream, compressedSize, outSize, null);

                    retVal = strmOutStream.ToArray();
                } // End Using newOutStream

            } // End Using newInStream

            return retVal;
        }
    }
}

