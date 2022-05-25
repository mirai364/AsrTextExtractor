using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace AsrTextExtractor
{
    class Program
    {
        static private Encoding ascii = Encoding.ASCII;
        static private Encoding unicode = Encoding.Unicode;

        static private byte[] gethash(CRCpolynomial type, bool InitialMask, bool FinalMask, byte[] data)
        {
            byte[] c;
            using (CRC crc = new CRC(type))
            {
                crc.InitialMask = InitialMask;
                crc.FinalMask = FinalMask;

                c = crc.ComputeHash(data);
            }
            return c;
        }

        static private List<string> unpack(string fileName, bool addCode = true)
        {
            List<string> textList = new List<string>();

            byte[] data = File.ReadAllBytes(fileName);
            using (MemoryStream mem = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(mem);
                char[] header = reader.ReadChars(4);
                uint Filesize = reader.ReadUInt32();
                uint Version = reader.ReadUInt32();
                uint Null = reader.ReadUInt32();
                uint num = reader.ReadUInt32();
                uint File_hash = reader.ReadUInt32();
                uint Text_string_size = reader.ReadUInt32();
                uint Language_id = reader.ReadUInt32();
                Console.WriteLine("HEADER: " + header);
                Console.WriteLine("Filesize: " + Filesize);
                Console.WriteLine("Version: " + Version);
                Console.WriteLine("Null: " + Null);
                Console.WriteLine("Number of strings: " + num);
                Console.WriteLine("File hash: " + File_hash);
                Console.WriteLine("Text string size [-Number of strings*8]: " + Text_string_size);
                Console.WriteLine("Language id: " + Language_id);
                Console.WriteLine();

                for (int i = 0; i < num; i++)
                {
                    byte[] hash = reader.ReadBytes(4);
                    byte[] length = reader.ReadBytes(4);
                    byte[] binData = reader.ReadBytes((int)BitConverter.ToUInt32(length) * 2);
                    string text = System.Text.Encoding.Unicode.GetString(binData);
                    if (addCode)
                    {
                        textList.Add(BitConverter.ToUInt32(hash) + "," + text);
                    } else
                    {
                        textList.Add(text);
                    }
                    /*
                    Console.WriteLine("String hash: " + BitConverter.ToUInt32(hash));
                    Console.WriteLine("String Length: " + BitConverter.ToUInt32(length));
                    Console.WriteLine("Unicode string (null terminated): " + text);
                    */
                }
            }
            return textList;
        }

        static void Main(string[] args)
        {
            // show usage if no args provided
            if (args.Length == 0)
            {
                Console.WriteLine();
                Console.WriteLine("Usage:");
                Console.WriteLine(" Unpack: AsrTextExtractor.exe -u <asr|en file> [<csv text file>]");
                Console.WriteLine(" Pack:   AsrTextExtractor.exe -p <csv text file> [<new asr|en file>]");
                Console.WriteLine();
                Console.WriteLine("options:");
                Console.WriteLine("  -u        Unpack option");
                Console.WriteLine("  -p        Pack option");
                Console.WriteLine();
                Console.WriteLine("arguments:");
                Console.WriteLine("  <asr file>      asr file path");
                Console.WriteLine("  <csv text file> csv text file path");
                Console.WriteLine("  <new asr file>  new asr file path");
                return;
            }

            string sourceFileName;
            string sourceFileName2;
            switch (args[0])
            {
                case "-c":
                    Console.WriteLine("Correspondence table");
                    sourceFileName = args[1];
                    sourceFileName2 = args[2];
                    Console.WriteLine("fileName : " + sourceFileName);
                    List<string> s1 = unpack(sourceFileName);
                    List<string> s2 = unpack(sourceFileName2, false);

                    List<string> lines = new List<string>();
                    for (int i=0;i<s1.Count;i++)
                    {
                        lines.Add(s1[i] + "," + s2[i]);
                    }
                    File.WriteAllLines(@"Correspondence.csv", lines, Encoding.Unicode);
                    break;
                case "-u":
                    Console.WriteLine("Unpack");
                    sourceFileName = args[1];
                    Console.WriteLine("fileName : " + sourceFileName);
                    List<string> s = unpack(sourceFileName);
                    File.WriteAllLines(@"pu.csv", s, Encoding.Unicode);
                    break;
                case "-p":
                    Console.WriteLine("Pack");
                    sourceFileName = args[1];
                    Console.WriteLine("fileName : " + sourceFileName);
                    break;
            }

        }
    }
}
