using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump02_TexDataEntry : LumpEntry {
    //struct dtexdata_t
    //{
    //	Vector  reflectivity;            // RGB reflectivity
    //	int     nameStringTableID;       // index into TexdataStringTable
    //	int     width, height;           // source image
    //	int     view_width, view_height;
    //};

    public float[] Reflectivity { get; set; } = new float[3];
    public int NameStringTableID { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public int ViewWidth { get; set; }
    public int ViewHeight { get; set; }

    public Lump02_TexDataEntry(BinaryReader reader) {
        for (var i = 0; i < 3; i++) {
            Reflectivity[i] = reader.ReadSingle();
        }

        NameStringTableID = reader.ReadInt32();
        Width = reader.ReadInt32();
        Height = reader.ReadInt32();
        ViewWidth = reader.ReadInt32();
        ViewHeight = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        for (var i = 0; i < 3; i++) {
            writer.Write(Reflectivity[i]);
        }

        writer.Write(NameStringTableID);
        writer.Write(Width);
        writer.Write(Height);
        writer.Write(ViewWidth);
        writer.Write(ViewHeight);

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class Lump02_TexData : BspLump {
    public const int ID = 2;
    public Lump02_TexData(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump02_TexDataEntry(reader);
    }
}