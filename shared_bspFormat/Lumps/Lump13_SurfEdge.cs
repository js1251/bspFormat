using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump13_SurfEdgeEntry : LumpEntry {
    //struct dsurfedge_t
    //{
    //    integer edgeIndex;  // |edgeIndex| = index into edge array, sign indicates direction (positive: first to second, negative: second to first)
    //};

    public int EdgeIndex { get; set; }

    public Lump13_SurfEdgeEntry(BinaryReader reader) {
        EdgeIndex = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(EdgeIndex);

        return stream.ToArray();
    }
}

public sealed class Lump13_SurfEdge : BspLump {
    public const int ID = 13;
    public Lump13_SurfEdge(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump13_SurfEdgeEntry(reader);
    }
}