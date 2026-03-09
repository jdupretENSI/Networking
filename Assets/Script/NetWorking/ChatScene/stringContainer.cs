using Unity.Netcode;

public struct stringContainer : INetworkSerializable
{
    public string SomeText;

    public stringContainer(string text) {
        SomeText = text;
    }
    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter {
        if (serializer.IsWriter) {
            serializer.GetFastBufferWriter().WriteValueSafe(SomeText);
        }
        else {
            serializer.GetFastBufferReader().ReadValueSafe(out SomeText);
        }
    }
}