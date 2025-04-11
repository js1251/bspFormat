using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump03_VertexEntry : LumpEntry {
    //struct dvertex_t
    //{
    //    float vertex[3];  // vertex coordinates
    //};

    public float[] Vertex { get; set; } = new float[3];

    public Lump03_VertexEntry(BinaryReader reader) {
        for (var i = 0; i < 3; i++) {
            Vertex[i] = reader.ReadSingle();
        }
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        for (var i = 0; i < 3; i++) {
            writer.Write(Vertex[i]);
        }

        return stream.ToArray();
    }
}

public sealed class Lump03_Vertex : BspLump {
    public const int ID = 3;
    public Lump03_Vertex(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump03_VertexEntry(reader);
    }
}