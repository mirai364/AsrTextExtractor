using System;

public struct CsvData
{
    public string command;
    public string sourceText;
    public string overrideText;

    public CsvData(string line)
    {
        string[] values = line.Split("\t");
        this.command =values[0];
        this.sourceText = values[1];

        this.overrideText = values[1];
        if (values.Length == 3)
        {
            this.overrideText = values[2];
        }
    }

    public CsvData(string command, string sourceText, string overrideText)
    {
        this.command = command;
        this.sourceText = sourceText;
        this.overrideText = overrideText;
    }

    public uint getCode()
    {
        return Convert.ToUInt32(this.command);
    }

    public string getCsvText(bool addCode = true)
    {
        return this.command + "\t" + this.sourceText + "\t" + this.overrideText;
    }

    public void show()
    {
        Console.WriteLine(this.getCsvText());
    }
}