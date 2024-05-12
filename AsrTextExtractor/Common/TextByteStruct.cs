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

        string text = overrideData.overrideText;
        text = text.Replace("<INPUT_FRONTEND_UPDOWN>", "\uE9FB");
        text = text.Replace("<INPUT_FRONTEND_LEFTRIGHT>", "\uE9FA");
        text = text.Replace("<INPUT_FRONTEND_MAP_SHOW_OBJECTIVES>", "\uE96E");
        text = text.Replace("<INPUT_FRONTEND_SELECT_UNK>", "\uE968");
        text = text.Replace("<INPUT_FRONTEND_ZOOM_OUT>", "\uE967");
        text = text.Replace("<INPUT_FRONTEND_ZOOM_IN>", "\uE966");
        text = text.Replace("<INPUT_FRONTEND_MAP_TOGGLE_ZOOM>", "\uE965");
        text = text.Replace("<INPUT_FRONTEND_MAP_TRACK>", "\uE964");
        text = text.Replace("<INPUT_FRONTEND_MAP_MY_POSITION>", "\uE963");
        text = text.Replace("<INPUT_TOGGLE_VOICE_CHAT>", "\uE89E");
        text = text.Replace("<INPUT_CAMERA_SWAP>", "\uE892");
        text = text.Replace("<INPUT_DECREASE_SCOPE_RANGE>", "\uE883");
        text = text.Replace("<INPUT_INCREASE_SCOPE_RANGE>", "\uE882");
        text = text.Replace("<INPUT_DEFUSE>", "\uE87E");
        text = text.Replace("<INPUT_DROP_DOWN>", "\uE87D");
        text = text.Replace("<INPUT_VIEW_COLLECTIBLE>", "\uE87C");
        text = text.Replace("<INPUT_TAKEDOWN>", "\uE879");
        text = text.Replace("<INPUT_SWAP_SECONDARY>", "\uE873");
        text = text.Replace("<INPUT_SEARCH_CORPSE>", "\uE872");
        text = text.Replace("<INPUT_CLOSE_MAP>", "\uE86B");
        text = text.Replace("<INPUT_RADIAL_MENU>", "\uE869");
        text = text.Replace("<INPUT_VOICE_CHAT>", "\uE863");
        text = text.Replace("<INPUT_BINOCULARS>", "\uE859");
        text = text.Replace("<INPUT_REVIVE_BUDDY>", "\uE858");
        text = text.Replace("<INPUT_TAG_ENVIRONMENT>", "\uE854");
        text = text.Replace("<INPUT_SCOPE_ZOOM_OUT>", "\uE853");
        text = text.Replace("<INPUT_SCOPE_ZOOM_IN>", "\uE852");
        text = text.Replace("<INPUT_ZOOM_OUT>", "\uE84E");
        text = text.Replace("<INPUT_ZOOM_IN>", "\uE84D");
        text = text.Replace("<INPUT_PICK_UP_WEAPON>", "\uE84C");
        text = text.Replace("<INPUT_PICK_UP_BODY>", "\uE84B");
        text = text.Replace("<INPUT_USE_COVER>", "\uE848");
        text = text.Replace("<INPUT_INTERACT>", "\uE847");
        text = text.Replace("<INPUT_EMPTY_LUNG>", "\uE846");
        text = text.Replace("<INPUT_SPAWN>", "\uE845");
        text = text.Replace("<INPUT_CAMERA_PREVIOUS>", "\uE844");
        text = text.Replace("<INPUT_CAMERA_NEXT>", "\uE843");
        text = text.Replace("<INPUT_MAP_OBJECTIVES>", "\uE840");
        text = text.Replace("<INPUT_TRAVERSE>", "\uE83F");
        text = text.Replace("<INPUT_USE_ITEM>", "\uE83B");
        text = text.Replace("<INPUT_SHOOT>", "\uE838");
        text = text.Replace("<INPUT_STANCE>", "\uE82A");
        text = text.Replace("<INPUT_OPEN_VOTE>", "\uE827");
        text = text.Replace("<INPUT_FRONTEND_SKIP_CUTSCENE>", "\uE82E");
        text = text.Replace("<INPUT_FRONTEND_RS>", "\uE81A");
        text = text.Replace("<INPUT_FRONTEND_RT>", "\uE80E");
        text = text.Replace("<INPUT_FRONTEND_LT>", "\uE80D");
        text = text.Replace("<INPUT_FRONTEND_RB>", "\uE80C");
        text = text.Replace("<INPUT_FRONTEND_LB>", "\uE80B");
        text = text.Replace("<INPUT_FRONTEND_START>", "\uE80A");
        text = text.Replace("<INPUT_FRONTEND_SELECT>", "\uE809");
        text = text.Replace("<INPUT_FRONTEND_LS>", "\uE808");
        text = text.Replace("<INPUT_FRONTEND_Y>", "\uE807");
        text = text.Replace("<INPUT_FRONTEND_X>", "\uE806");
        text = text.Replace("<INPUT_FRONTEND_RIGHT>", "\uE805");
        text = text.Replace("<INPUT_FRONTEND_LEFT>", "\uE804");
        text = text.Replace("<INPUT_FRONTEND_DOWN>", "\uE803");
        text = text.Replace("<INPUT_FRONTEND_UP>", "\uE802");
        text = text.Replace("<INPUT_FRONTEND_B>", "\uE801");
        text = text.Replace("<INPUT_FRONTEND_A>", "\uE800");
        text = text.Replace("<END>", "\u0000");
        text = text.Replace("<TAB>", "\u0009");
        text = text.Replace("<NR>", "\u000D");
        text = text.Replace("<NL>", "\u000A");
        text = text.Replace("<HIGHLIGHT_SET_END>", "\uE002");
        text = text.Replace("<HIGHLIGHT_RGB_SET_START>", "\uE001\u0003");
        text = text.Replace("<HIGHLIGHT_SET_START>", "\uE001\u0002");
        text = text.Replace("<HIGHLIGHT_END>", "\uE001\u0001\uE002");

        this.data = System.Text.Encoding.Unicode.GetBytes(text);
        this.length = BitConverter.GetBytes((uint)text.Length);
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
        text = text.Replace("\uE001\u0001\uE002", "<HIGHLIGHT_END>");
        text = text.Replace("\uE001\u0002", "<HIGHLIGHT_SET_START>");
        text = text.Replace("\uE001\u0003", "<HIGHLIGHT_RGB_SET_START>");
        text = text.Replace("\uE002", "<HIGHLIGHT_SET_END>");
        text = text.Replace("\n", "<NL>");
        text = text.Replace("\r", "<NR>");
        text = text.Replace("\t", "<TAB>");
        text = text.Replace("\0", "<END>");
        text = text.Replace("\uE800", "<INPUT_FRONTEND_A>");
        text = text.Replace("\uE801", "<INPUT_FRONTEND_B>");
        text = text.Replace("\uE802", "<INPUT_FRONTEND_UP>");
        text = text.Replace("\uE803", "<INPUT_FRONTEND_DOWN>");
        text = text.Replace("\uE804", "<INPUT_FRONTEND_LEFT>");
        text = text.Replace("\uE805", "<INPUT_FRONTEND_RIGHT>");
        text = text.Replace("\uE806", "<INPUT_FRONTEND_X>");
        text = text.Replace("\uE807", "<INPUT_FRONTEND_Y>");
        text = text.Replace("\uE808", "<INPUT_FRONTEND_LS>");
        text = text.Replace("\uE809", "<INPUT_FRONTEND_SELECT>");
        text = text.Replace("\uE80A", "<INPUT_FRONTEND_START>");
        text = text.Replace("\uE80B", "<INPUT_FRONTEND_LB>");
        text = text.Replace("\uE80C", "<INPUT_FRONTEND_RB>");
        text = text.Replace("\uE80D", "<INPUT_FRONTEND_LT>");
        text = text.Replace("\uE80E", "<INPUT_FRONTEND_RT>");
        text = text.Replace("\uE81A", "<INPUT_FRONTEND_RS>");
        text = text.Replace("\uE82E", "<INPUT_FRONTEND_SKIP_CUTSCENE>");
        text = text.Replace("\uE827", "<INPUT_OPEN_VOTE>");
        text = text.Replace("\uE82A", "<INPUT_STANCE>");
        text = text.Replace("\uE838", "<INPUT_SHOOT>");
        text = text.Replace("\uE83B", "<INPUT_USE_ITEM>");
        text = text.Replace("\uE83F", "<INPUT_TRAVERSE>");
        text = text.Replace("\uE840", "<INPUT_MAP_OBJECTIVES>");
        text = text.Replace("\uE843", "<INPUT_CAMERA_NEXT>");
        text = text.Replace("\uE844", "<INPUT_CAMERA_PREVIOUS>");
        text = text.Replace("\uE845", "<INPUT_SPAWN>");
        text = text.Replace("\uE846", "<INPUT_EMPTY_LUNG>");
        text = text.Replace("\uE847", "<INPUT_INTERACT>");
        text = text.Replace("\uE848", "<INPUT_USE_COVER>");
        text = text.Replace("\uE84B", "<INPUT_PICK_UP_BODY>");
        text = text.Replace("\uE84C", "<INPUT_PICK_UP_WEAPON>");
        text = text.Replace("\uE84D", "<INPUT_ZOOM_IN>");
        text = text.Replace("\uE84E", "<INPUT_ZOOM_OUT>");
        text = text.Replace("\uE852", "<INPUT_SCOPE_ZOOM_IN>");
        text = text.Replace("\uE853", "<INPUT_SCOPE_ZOOM_OUT>");
        text = text.Replace("\uE854", "<INPUT_TAG_ENVIRONMENT>");
        text = text.Replace("\uE858", "<INPUT_REVIVE_BUDDY>");
        text = text.Replace("\uE859", "<INPUT_BINOCULARS>");
        text = text.Replace("\uE863", "<INPUT_VOICE_CHAT>");
        text = text.Replace("\uE869", "<INPUT_RADIAL_MENU>");
        text = text.Replace("\uE86B", "<INPUT_CLOSE_MAP>");
        text = text.Replace("\uE872", "<INPUT_SEARCH_CORPSE>");
        text = text.Replace("\uE873", "<INPUT_SWAP_SECONDARY>");
        text = text.Replace("\uE879", "<INPUT_TAKEDOWN>");
        text = text.Replace("\uE87C", "<INPUT_VIEW_COLLECTIBLE>");
        text = text.Replace("\uE87D", "<INPUT_DROP_DOWN>");
        text = text.Replace("\uE87E", "<INPUT_DEFUSE>");
        text = text.Replace("\uE882", "<INPUT_INCREASE_SCOPE_RANGE>");
        text = text.Replace("\uE883", "<INPUT_DECREASE_SCOPE_RANGE>");
        text = text.Replace("\uE892", "<INPUT_CAMERA_SWAP>");
        text = text.Replace("\uE89E", "<INPUT_TOGGLE_VOICE_CHAT>");
        text = text.Replace("\uE963", "<INPUT_FRONTEND_MAP_MY_POSITION>");
        text = text.Replace("\uE964", "<INPUT_FRONTEND_MAP_TRACK>");
        text = text.Replace("\uE965", "<INPUT_FRONTEND_MAP_TOGGLE_ZOOM>");
        text = text.Replace("\uE966", "<INPUT_FRONTEND_ZOOM_IN>");
        text = text.Replace("\uE967", "<INPUT_FRONTEND_ZOOM_OUT>");
        text = text.Replace("\uE968", "<INPUT_FRONTEND_SELECT_UNK>");
        text = text.Replace("\uE96E", "<INPUT_FRONTEND_MAP_SHOW_OBJECTIVES>");
        text = text.Replace("\uE9FA", "<INPUT_FRONTEND_LEFTRIGHT>");
        text = text.Replace("\uE9FB", "<INPUT_FRONTEND_UPDOWN>");

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