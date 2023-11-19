namespace shared_bspFormat;

public abstract class LumpEntry {
    public abstract byte[] ToBytes();
}

public abstract class BspLump {
    public List<LumpEntry> Entries { get; } = new();
    public byte[] Padding { get; set; }

    public BspLump(byte[] bytes) {
        Parse(bytes);
    }

    public virtual byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        foreach (var entry in Entries) {
            writer.Write(entry.ToBytes());
        }

        if (Padding is not null) {
            writer.Write(Padding);
        }

        writer.Close();
        return stream.ToArray();
    }

    private void Parse(byte[] bytes) {
        var reader = new BinaryReader(new MemoryStream(bytes));
        
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