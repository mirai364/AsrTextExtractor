using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Transactions;

namespace AsrTextExtractor
{
    class Program
    {
        static private Dictionary<ulong, string> Unpack(string fileName, bool addCode = true)
        {
            Dictionary<ulong, string> textList = new Dictionary<ulong, string>();

            byte[] data = File.ReadAllBytes(fileName);
            using (MemoryStream mem = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(mem);
                byte[] asure = reader.ReadBytes(8);
                if (BitConverter.ToUInt64(asure) != 2314885811372651329)
                {
                    Usage();
                    Console.WriteLine();
                    Console.WriteLine("Unauthorized formats : " + fileName);
                    Environment.Exit(-1);

                }
                byte[] header = reader.ReadBytes(4);
                if (BitConverter.ToUInt32(header) != 1415074888)
                {
                    Usage();
                    Console.WriteLine();
                    Console.WriteLine("Unauthorized formats : " + fileName);
                    Environment.Exit(-1);
                }
                uint Filesize = reader.ReadUInt32();
                uint Version = reader.ReadUInt32();
                uint Null = reader.ReadUInt32();
                uint num = reader.ReadUInt32();
                uint File_hash = reader.ReadUInt32();
                uint Text_string_size = reader.ReadUInt32();
                uint Language_id = reader.ReadUInt32();
                Console.WriteLine("HEADER: " + Encoding.ASCII.GetString(header));
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

                    TextByte textByte = new TextByte(hash, length, binData);
                    textList.Add(BitConverter.ToUInt32(textByte.code), textByte.getCsvText(addCode));
                }
            }
            return textList;
        }

        static private void OverrideFile(string overrideFileName, string sourceFileName, string outputFileName, bool force = false)
        {
            List<TextByte> textByteList = new List<TextByte>();
            Dictionary<uint, CsvData> overrideList = new Dictionary<uint, CsvData>();

            // Read CSV
            using (StreamReader reader = new StreamReader(sourceFileName, Encoding.Unicode))
            {
                while (!reader.EndOfStream)
                {
                    CsvData csvData = new CsvData(reader.ReadLine());
                    overrideList.Add(csvData.getCode(), csvData);
                }
            }

            byte[] data = File.ReadAllBytes(overrideFileName);
            using (MemoryStream mem = new MemoryStream(data))
            {
                BinaryReader reader = new BinaryReader(mem);
                byte[] asure = reader.ReadBytes(8);
                if (BitConverter.ToUInt64(asure) != 2314885811372651329)
                {
                    Usage();
                    Console.WriteLine();
                    Console.WriteLine("Unauthorized formats : " + overrideFileName);
                    Environment.Exit(-1);

                }
                byte[] header = reader.ReadBytes(4);
                if (BitConverter.ToUInt32(header) != 1415074888)
                {
                    Usage();
                    Console.WriteLine();
                    Console.WriteLine("Unauthorized formats : " + overrideFileName);
                    Environment.Exit(-1);
                }
                byte[] Filesize = reader.ReadBytes(4);
                byte[] Version = reader.ReadBytes(4);
                byte[] Null = reader.ReadBytes(4);
                byte[] Number_of_strings = reader.ReadBytes(4);
                byte[] File_hash = reader.ReadBytes(4);
                byte[] Text_string_size = reader.ReadBytes(4);
                byte[] Language_id = reader.ReadBytes(4);
                int num = (int)BitConverter.ToUInt32(Number_of_strings);

                int size = -num * 8;
                for (int i = 0; i < num; i++)
                {
                    byte[] hash = reader.ReadBytes(4);
                    byte[] length = reader.ReadBytes(4);
                    byte[] binData = reader.ReadBytes((int)BitConverter.ToUInt32(length) * 2);

                    TextByte textByte = new TextByte(hash, length, binData);
                    if (overrideList.ContainsKey(textByte.getUintCode()))
                    {
                        textByte.overrideData(overrideList[textByte.getUintCode()], force);
                    }

                    size += textByte.getALLBytes().Length;
                    textByteList.Add(textByte);
                }

                // write
                var writer = new BinaryWriter(new FileStream(outputFileName, FileMode.Create));
                try
                {
                    writer.Write(asure);
                    writer.Write(header);
                    writer.Write(Filesize);
                    writer.Write(Version);
                    writer.Write(Null);
                    writer.Write(Number_of_strings);
                    writer.Write(File_hash);
                    writer.Write((uint)size);
                    writer.Write(Language_id);
                    for(int i=0;i< textByteList.Count;i++)
                    {
                        writer.Write(textByteList[i].getALLBytes());
                    }

                    var baseStream = reader.BaseStream;
                    while (baseStream.Position != baseStream.Length)
                    {
                        writer.Write(reader.ReadByte());
                    }
                }
                finally
                {
                    writer.Close();
                }
            }
        }

        static void Usage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine(" Comparison: AsrTextExtractor.exe -c <asr|en file> <asr|en file> [<csv text file>]");
            Console.WriteLine(" Unpack    : AsrTextExtractor.exe -u <asr|en file> [<csv text file>]");
            Console.WriteLine(" Override  : AsrTextExtractor.exe -o <asr|en file> <csv text file> [<new asr|en file>]");
            Console.WriteLine();
            Console.WriteLine("options:");
            Console.WriteLine("  -c        Create Comparison table option");
            Console.WriteLine("  -u        Unpack option");
            Console.WriteLine("  -o        Override option");
            Console.WriteLine("  -fo       Override Force option");
            Console.WriteLine();
            Console.WriteLine("arguments:");
            Console.WriteLine("  <asr file>      asr file path");
            Console.WriteLine("  <csv text file> csv text file path");
            Console.WriteLine("  <new asr file>  new asr file path");
        }

        static void Main(string[] args)
        {
            // show usage if no args provided
            if (args.Length == 0)
            {
                Usage();
                return;
            }

            switch (args[0])
            {
                case "-c":
                    if (args.Length < 3 || args.Length > 4)
                    {
                        Usage();
                        return;
                    }
                    Console.WriteLine("Comparison table");
                    string sourceFileName = args[1];
                    string sourceFileName2 = args[2];
                    string outputFileName = Path.GetFileNameWithoutExtension(args[1]) + ".csv";
                    if (args.Length == 4)
                    {
                        outputFileName = args[3];
                    }

                    Console.WriteLine("fileName : " + sourceFileName);
                    Dictionary<ulong, string> s1 = Unpack(sourceFileName);
                    Dictionary<ulong, string> s2 = Unpack(sourceFileName2, false);

                    List<string> lines = new List<string>();
                    foreach (KeyValuePair<ulong, string> kvp in s1)
                    {
                        ulong id = kvp.Key;
                        string name = kvp.Value;
                        if (s2.ContainsKey(key: id) == true)
                        {
                            lines.Add(name + "\t" + s2[id]);
                        }
                        else
                        {
                            lines.Add(name + "\t" + name);
                        }
                    }
                    File.WriteAllLines(outputFileName, lines, Encoding.Unicode);
                    break;
                case "-u":
                    if (args.Length < 2 || args.Length > 3)
                    {
                        Usage();
                        return;
                    }
                    Console.WriteLine("Unpack");
                    sourceFileName = args[1];
                    outputFileName = Path.GetFileNameWithoutExtension(args[1]) + ".csv";
                    if (args.Length == 3)
                    {
                        outputFileName = args[2];
                    }
                    Console.WriteLine("fileName : " + sourceFileName);
                    Dictionary<ulong, string> s = Unpack(sourceFileName);
                    File.WriteAllLines(outputFileName, s.Values, Encoding.Unicode);
                    break;
                case "-o":
                    if (args.Length < 3 || args.Length > 4)
                    {
                        Usage();
                        return;
                    }
                    Console.WriteLine("Override");
                    string overrideFileName = args[1];
                    sourceFileName = args[2];
                    outputFileName = args[1];
                    if (args.Length == 4)
                    {
                        outputFileName = args[3];
                    }
                    else
                    {
                        if (! File.Exists(args[1] + "_back"))
                        {
                            File.Copy(args[1], args[1] + "_back");
                            Console.WriteLine("backupFile : " + args[1] + "_back");
                        }
                    }
                    Console.WriteLine("overrideFileName : " + overrideFileName);
                    Console.WriteLine("sourceFileName : " + sourceFileName);
                    OverrideFile(overrideFileName, sourceFileName, outputFileName);
                    break;
                case "-fo":
                    if (args.Length < 3 || args.Length > 4)
                    {
                        Usage();
                        return;
                    }
                    Console.WriteLine("Override");
                    overrideFileName = args[1];
                    sourceFileName = args[2];
                    outputFileName = args[1];
                    if (args.Length == 4)
                    {
                        outputFileName = args[3];
                    }
                    else
                    {
                        if (!File.Exists(args[1] + "_back"))
                        {
                            File.Copy(args[1], args[1] + "_back");
                            Console.WriteLine("backupFile : " + args[1] + "_back");
                        }
                    }
                    Console.WriteLine("overrideFileName : " + overrideFileName);
                    Console.WriteLine("sourceFileName : " + sourceFileName);
                    OverrideFile(overrideFileName, sourceFileName, outputFileName, true);
                    break;
            }

        }
    }
}
