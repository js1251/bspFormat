using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump12_EdgeEntry : LumpEntry {
    //struct dedge_t
    //{
    //    unsigned short v[2];  // vertex indices
    //};

    public ushort[] V { get; set; } = new ushort[2];

    public Lump12_EdgeEntry(BinaryReader reader) {
        for (var i = 0; i < 2; i++) {
            V[i] = reader.ReadUInt16();
        }
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        for (var i = 0; i < 2; i++) {
            writer.Write(V[i]);
        }

        return stream.ToArray();
    }
}

public sealed class Lump12_Edge : BspLump {
    public const int ID = 12;
    public Lump12_Edge(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump12_EdgeEntry(reader);
    }
}