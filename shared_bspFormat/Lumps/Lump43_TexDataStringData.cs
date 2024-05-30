using System.IO;
using System.Text;

namespace shared_bspFormat.Lumps;

public class Lump43_TexDataStringDataEntry : LumpEntry {
    public string MaterialName { get; }

    public Lump43_TexDataStringDataEntry(string materialName) {
        MaterialName = materialName;
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(Encoding.ASCII.GetBytes(MaterialName));

        // add null terminator
        writer.Write(new byte[] { 0x00 });

        writer.Close();
        return stream.ToArray();
    }
}

public class Lump43_TexDataStringData : BspLump {
    public const int ID = 43;
    public Lump43_TexDataStringData(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        if (reader.BaseStream.Position >= reader.BaseStream.Length - 1) {
            return null;
        }

        // create a byte buffer stream
        var stream = new MemoryStream();

        // read bytes until we hit a null terminator
        byte b;
        while ((b = reader.ReadByte()) is not 0) {
            stream.WriteByte(b);
        }

        // TODO: what is this?
        if (stream.Length == 1 && stream.GetBuffer()[0] == 0) {
            return null;
        }

        // convert the byte buffer to a string
        return new Lump43_TexDataStringDataEntry(Encoding.ASCII.GetString(stream.ToArray()));
    }
}