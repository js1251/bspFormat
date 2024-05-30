using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump06_TexInfoEntry : LumpEntry {
    //struct texinfo_t
    //{
    //    float textureVecs[2][4];  // [s/t][xyz offset]
    //    float lightmapVecs[2][4]; // [s/t][xyz offset] - length is in units of texels/area
    //    int flags;                // miptex flags overrides
    //    int texdata;              // Pointer to texture name, size, etc.
    //}

    public float[][] TextureVecs { get; set; } = new float[2][];
    public float[][] LightmapVecs { get; set; } = new float[2][];
    public int Flags { get; set; }
    public int TexData { get; set; }

    public Lump06_TexInfoEntry(BinaryReader reader) {
        for (var i = 0; i < 2; i++) {
            TextureVecs[i] = new float[4];
            LightmapVecs[i] = new float[4];
            for (var j = 0; j < 4; j++) {
                TextureVecs[i][j] = reader.ReadSingle();
                LightmapVecs[i][j] = reader.ReadSingle();
            }
        }

        Flags = reader.ReadInt32();
        TexData = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        for (var i = 0; i < 2; i++) {
            for (var j = 0; j < 4; j++) {
                writer.Write(TextureVecs[i][j]);
                writer.Write(LightmapVecs[i][j]);
            }
        }

        writer.Write(Flags);
        writer.Write(TexData);

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class Lump06_TexInfo : BspLump {
    public const int ID = 6;
    public Lump06_TexInfo(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump06_TexInfoEntry(reader);
    }
}