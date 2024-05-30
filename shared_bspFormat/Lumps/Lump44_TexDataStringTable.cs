using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump44_TexDataStringTableEntry : LumpEntry {

    public int Offset { get; set; }

    public Lump44_TexDataStringTableEntry(BinaryReader reader) {
        Offset = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(Offset);

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class Lump44_TexDataStringTable : BspLump {
    public const int ID = 44;
    public Lump44_TexDataStringTable(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump44_TexDataStringTableEntry(reader);
    }
}