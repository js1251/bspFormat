using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump01_PlaneEntry : LumpEntry {
    //struct dplane_t
    //{
    //    Vector normal;   // normal vector
    //    float dist;     // distance from origin
    //    int type;     // plane axis identifier
    //};

    public float[] Normal { get; set; } = new float[3];
    public float Dist { get; set; }
    public int Type { get; set; }

    public Lump01_PlaneEntry(BinaryReader reader) {
        for (var i = 0; i < 3; i++) {
            Normal[i] = reader.ReadSingle();
        }

        Dist = reader.ReadSingle();
        Type = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        for (var i = 0; i < 3; i++) {
            writer.Write(Normal[i]);
        }

        writer.Write(Dist);
        writer.Write(Type);

        return stream.ToArray();
    }
}

public sealed class Lump01_Planes : BspLump {
    public const int ID = 1;
    public Lump01_Planes(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump01_PlaneEntry(reader);
    }
}