using Unity.Netcode;

public struct ChatBoxMessage: INetworkSerializable
{
    public string PlayerTarget;
    public string PlayerFrom;
    public int TextColor;
    public string Text;

    public ChatBoxMessage(string playerTarget, int textColor, string text, string playerFrom)
    {
        PlayerTarget = playerTarget;
        TextColor = textColor;
        Text = text;
        PlayerFrom = playerFrom;
    }

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        serializer.SerializeValue(ref PlayerTarget);
        serializer.SerializeValue(ref PlayerFrom);
        serializer.SerializeValue(ref TextColor);
        serializer.SerializeValue(ref Text);
        
        // if (serializer.IsWriter) {
        //     serializer.GetFastBufferWriter().WriteValueSafe(PlayerTarget);
        //     serializer.GetFastBufferWriter().WriteValueSafe(PlayerFrom);
        //     serializer.GetFastBufferWriter().WriteValueSafe(TextColor);
        //     serializer.GetFastBufferWriter().WriteValueSafe(Text);
        //     
        // }
        // else {
        //     serializer.GetFastBufferReader().ReadValueSafe(out PlayerTarget);
        //     serializer.GetFastBufferReader().ReadValueSafe(out PlayerFrom);
        //     serializer.GetFastBufferReader().ReadValueSafe(out TextColor);
        //     serializer.GetFastBufferReader().ReadValueSafe(out Text);
        // }
    }
}