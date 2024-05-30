using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump18_BrushesEntry : LumpEntry {
    //struct dbrush_t
    //{
    //    int firstside;     // first brushside
    //    int numsides;      // number of brushsides
    //    int contents;      // contents flags
    //};
    public int FirstSide { get; set; }
    public int NumSides { get; set; }
    public int Contents { get; set; }

    public Lump18_BrushesEntry(BinaryReader reader) {
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

public sealed class Lump18_Brushes : BspLump {
    public const int ID = 18;
    public Lump18_Brushes(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump18_BrushesEntry(reader);
    }
}