namespace UhgeEngine.Core.Assets;

public abstract class Asset
{
    public string? UUID { get; internal set; }

    public abstract byte[] SerializeToBytes();
    public abstract string SerializeToString();

    public abstract void DeserializeFrom(byte[] bytes);
    public abstract void DeserializeFrom(string str);
}