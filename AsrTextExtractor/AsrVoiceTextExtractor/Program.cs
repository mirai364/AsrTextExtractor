using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AsrVoiceTextExtractor
{
    internal class Program
    {
        static private List<string> Unpack(string fileName, int sourceNumber = 0, int overrideNumber = 7)
        {
            List<string> csvData = new List<string>();
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
                while (true)
                {
                    while (true)
                    {
                        byte[] header = reader.ReadBytes(4);
                        if (header.Length < 4)
                        {
                            return csvData;
                        }
                        //Console.WriteLine(BitConverter.ToString(header) + ": " + BitConverter.ToUInt32(header));
                        if (BitConverter.ToUInt32(header) != 1313623108) // DLLN
                        {
                            reader.BaseStream.Position -= 3;
                            continue;
                        }
                        break;
                    }
                    uint Filesize = reader.ReadUInt32();
                    uint Version = reader.ReadUInt32();
                    uint Null = reader.ReadUInt32();
                    Console.WriteLine("Filesize: " + Filesize);
                    Console.WriteLine("Version: " + Version);
                    Console.WriteLine("Null: " + Null);

                    long start = reader.BaseStream.Position;
                    int count = 0;
                    while (true)
                    {
                        byte[] bin = reader.ReadBytes(4);
                        count += 4;
                        if (bin[3] == 0)
                        {
                            break;
                        }
                    }
                    while (true)
                    {
                        byte bin = reader.ReadByte();
                        if (bin != 0)
                        {
                            break;
                        }
                        count++; 
                    }
                    reader.BaseStream.Position = start;
                    byte[] command = reader.ReadBytes(count);
                    Console.WriteLine(Encoding.ASCII.GetString(command));
                    byte[] code1 = reader.ReadBytes(4);
                    byte[] code2 = reader.ReadBytes(4);
                    byte[] timestamp1 = reader.ReadBytes(4);
                    reader.ReadByte();
                    byte[] timestamp2 = reader.ReadBytes(4);
                    reader.ReadUInt32();
                    byte[] command2 = reader.ReadBytes(4);
                    Console.WriteLine("code1: " + BitConverter.ToUInt32(code1));
                    Console.WriteLine("code2: " + BitConverter.ToUInt32(code2));
                    Console.WriteLine("timestamp1: " + BitConverter.ToUInt32(timestamp1));
                    Console.WriteLine("timestamp2: " + BitConverter.ToUInt32(timestamp2));
                    Console.WriteLine("command2: " + BitConverter.ToUInt32(command2));

                    string sourceText = "";
                    string overrideText = "";
                    switch (Version)
                    {
                        case 5:
                            byte[] Num_of_string = reader.ReadBytes(4);
                            int num = (int)BitConverter.ToUInt32(Num_of_string);
                            for (int i = 0; i < num; i++)
                            {
                                byte[] lang = reader.ReadBytes(4);
                                byte[] length = reader.ReadBytes(4);
                                byte[] binData = reader.ReadBytes((int)BitConverter.ToUInt32(length) * 2);

                                if (BitConverter.ToUInt32(lang) == sourceNumber)
                                {
                                    sourceText = Encoding.Unicode.GetString(binData);
                                }
                                if (BitConverter.ToUInt32(lang) == overrideNumber)
                                {
                                    overrideText = Encoding.Unicode.GetString(binData);
                                }
                            }
                            break;
                        case 4:
                            {
                                byte[] length = reader.ReadBytes(4);
                                byte[] binData = reader.ReadBytes((int)BitConverter.ToUInt32(length) * 2);
                                sourceText = overrideText = Encoding.Unicode.GetString(binData);
                            }
                            break;
                    }
                    CsvData tmp = new CsvData(Encoding.ASCII.GetString(command), sourceText, overrideText);
                    tmp.show();
                    csvData.Add(tmp.getCsvText());
                    Console.WriteLine();
                }
            }
        }

        static private void OverrideFile(string overrideFileName, string sourceFileName, string outputFileName)
        {
            Dictionary<string, CsvData> overrideList = new Dictionary<string, CsvData>();

            // Read CSV
            using (StreamReader reader = new StreamReader(sourceFileName, Encoding.Unicode))
            {
                while (!reader.EndOfStream)
                {
                    CsvData csvData = new CsvData(reader.ReadLine());
                    overrideList.Add(csvData.command, csvData);
                }
            }

            var writer = new BinaryWriter(new FileStream(outputFileName, FileMode.Create));
            try
            {
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
                    writer.Write(asure);

                    while (true)
                    {
                        while (true)
                        {
                            byte[] header = reader.ReadBytes(4);
                            if (header.Length < 4)
                            {
                                writer.Write(header);
                                return;
                            }
                            //Console.WriteLine(BitConverter.ToString(header) + ": " + BitConverter.ToUInt32(header));
                            if (BitConverter.ToUInt32(header) != 1313623108) // DLLN
                            {
                                writer.Write(header[0]);
                                reader.BaseStream.Position -= 3;
                                continue;
                            }
                            writer.Write(header);
                            break;
                        }
                        reader.ReadBytes(4);
                        byte[] Version = reader.ReadBytes(4);
                        if (BitConverter.ToUInt32(Version) != 4)
                        {
                            Usage();
                            Console.WriteLine();
                            Console.WriteLine("Unauthorized formats : " + overrideFileName);
                            Environment.Exit(-1);
                        }
                        byte[] Null = reader.ReadBytes(4);

                        long start = reader.BaseStream.Position;
                        int count = 0;
                        while (true)
                        {
                            byte[] bin = reader.ReadBytes(4);
                            count += 4;
                            if (bin[3] == 0)
                            {
                                break;
                            }
                        }
                        while (true)
                        {
                            byte bin = reader.ReadByte();
                            if (bin != 0)
                            {
                                break;
                            }
                            count++;
                        }
                        reader.BaseStream.Position = start;
                        byte[] command = reader.ReadBytes(count);
                        Console.WriteLine(Encoding.ASCII.GetString(command));

                        byte[] code = reader.ReadBytes(4 + 4 + 4 + 1 + 4 + 4 + 4);
                        byte[] length = reader.ReadBytes(4);
                        byte[] binData = reader.ReadBytes((int)BitConverter.ToUInt32(length) * 2);
                        TextByte textByte = new TextByte(code, length, binData);
                        if (overrideList.ContainsKey(Encoding.ASCII.GetString(command)))
                        {
                            textByte.overrideData(overrideList[Encoding.ASCII.GetString(command)], true);
                        }

                        int size = 4 + 4 + Version.Length + Null.Length + command.Length + textByte.getALLBytes().Length;
                        writer.Write(BitConverter.GetBytes((uint)size));
                        writer.Write(Version);
                        writer.Write(Null);
                        writer.Write(command);
                        writer.Write(textByte.getALLBytes());
                    }
                }
            }
            finally
            {
                writer.Close();
            }
        }

        static void Usage()
        {
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine(" Unpack    : AsrVoiceTextExtractor.exe -u <asr|en file> [<csv text file>]");
            Console.WriteLine(" Override  : AsrVoiceTextExtractor.exe -o <asr|en file> <csv text file> [<new asr|en file>]");
            Console.WriteLine();
            Console.WriteLine("options:");
            Console.WriteLine("  -u        Unpack option");
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
                case "-u":
                    if (args.Length < 2 || args.Length > 3)
                    {
                        Usage();
                        return;
                    }
                    Console.WriteLine("Unpack");
                    string sourceFileName = args[1];
                    string outputFileName = "output.csv";
                    if (args.Length == 3)
                    {
                        outputFileName = args[2];
                    }
                    Console.WriteLine("fileName : " + sourceFileName);
                    List<string> s = Unpack(sourceFileName);
                    File.WriteAllLines(outputFileName, s, Encoding.Unicode);
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
                    outputFileName = "output.bin";
                    if (args.Length == 4)
                    {
                        outputFileName = args[3];
                    }
                    Console.WriteLine("overrideFileName : " + overrideFileName);
                    Console.WriteLine("sourceFileName : " + sourceFileName);
                    OverrideFile(overrideFileName, sourceFileName, outputFileName);
                    break;
            }

        }
    }
}
