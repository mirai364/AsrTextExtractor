using System;

public struct OverrideData
{
    public uint code;
    public string sourceText;
    public string overrideText;

    public OverrideData(string line)
    {
        string[] values = line.Split("\t");
        this.code = Convert.ToUInt32(values[0]);
        this.sourceText = values[1];

        this.overrideText = values[1];
        if (values.Length == 3)
        {
            this.overrideText = values[2];
        }
    }
}