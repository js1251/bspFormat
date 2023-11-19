namespace shared_bspFormat.Lumps;

public sealed class DefaultLumpEntry : LumpEntry {
    private readonly byte[] _bytes;

    public DefaultLumpEntry(byte[] bytes) {
        _bytes = bytes;
    }

    public override byte[] ToBytes() {
        return _bytes;
    }
}

public sealed class DefaultBspLump : BspLump {
    public DefaultBspLump(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new DefaultLumpEntry(reader.ReadBytes(bytesToRead));
    }
}