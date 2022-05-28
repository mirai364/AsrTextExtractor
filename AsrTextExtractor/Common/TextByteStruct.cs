using System;

public struct TextByte
{
    public byte[] code;
    public byte[] length;
    public byte[] data;

    public TextByte(byte[] code, byte[] length, byte[] data)
    {
        this.code = code;
        this.length = length;
        this.data = data;
    }

    public TextByte overrideData(CsvData overrideData, bool force)
    {
        if (this.getText() != overrideData.sourceText && force == false)
        {
            return this;
        }

        this.length = BitConverter.GetBytes((uint)overrideData.overrideText.Length);
        this.data = System.Text.Encoding.Unicode.GetBytes(overrideData.overrideText);
        return this;
    }

    public byte[] getALLBytes()
    {
        byte[] mergedArray = new byte[code.Length + length.Length + data.Length];
        Array.Copy(code, mergedArray, code.Length);
        Array.Copy(length, 0, mergedArray, code.Length, length.Length);
        Array.Copy(data, 0, mergedArray, code.Length + length.Length, data.Length);
        return mergedArray;
    }

    public string getText()
    {
        string text = System.Text.Encoding.Unicode.GetString(this.data);
        text = text.Replace("\n", "\\n");
        text = text.Replace("\r", "\\r");
        text = text.Replace("\t", "\\t");
        return text;
    }

    public uint getUintCode()
    {
        return BitConverter.ToUInt32(this.code);
    }

    public uint getUintLength()
    {
        return BitConverter.ToUInt32(this.length);
    }

    public string getCsvText(bool addCode = true)
    {
        if (addCode)
        {
            return BitConverter.ToUInt32(this.code) + "\t" + this.getText();
        }

        return this.getText();
    }

    public void show()
    {
        Console.WriteLine("String hash: " + this.getUintCode());
        Console.WriteLine("String Length: " + this.getUintLength());
        Console.WriteLine("Unicode string (null terminated): " + this.getText());
        Console.WriteLine("byte: " + BitConverter.ToString(this.getALLBytes()));
        Console.WriteLine();
    }
}