namespace shared_bspFormat.Lumps;

public sealed class BrushLumpEntry : LumpEntry {
    //struct dbrush_t
    //{
    //    int firstside;     // first brushside
    //    int numsides;      // number of brushsides
    //    int contents;      // contents flags
    //};
    public int FirstSide { get; set; }
    public int NumSides { get; set; }
    public int Contents { get; set; }

    public BrushLumpEntry(BinaryReader reader) {
        FirstSide = reader.ReadInt32();
        NumSides = reader.ReadInt32();
        Contents = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(FirstSide);
        writer.Write(NumSides);
        writer.Write(Contents);

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class BrushBspLump : BspLump {
    public const int ID = 18;
    public BrushBspLump(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new BrushLumpEntry(reader);
    }
}