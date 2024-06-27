using System.Collections.Generic;
using System.IO;
using System.Text;

namespace shared_bspFormat;

public abstract class LumpEntry {
    public abstract byte[] ToBytes();
}

public abstract class BspLump {
    public List<LumpEntry> Entries { get; } = [];
    public int Padding { get; set; }

    public BspLump(byte[] bytes) {
        Parse(bytes);

        Padding = (4 - ToBytes().Length % 4) % 4;
    }

    public virtual byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        foreach (var entry in Entries) {
            writer.Write(entry.ToBytes());
        }

        writer.Close();
        return stream.ToArray();
    }

    private void Parse(byte[] bytes) {
        var reader = new BinaryReader(new MemoryStream(bytes), Encoding.GetEncoding("ISO-8859-1"));

        while (true) {
            var entry = ProvideEntry(reader);
            if (entry is null) {
                break;
            }

            Entries.Add(entry);
        }
    }

    protected abstract LumpEntry ProvideEntry(BinaryReader reader);
}