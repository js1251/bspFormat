namespace shared_bspFormat;

public abstract class LumpEntry {
    public abstract byte[] ToBytes();
}

public abstract class BspLump {
    public List<LumpEntry> Entries { get; } = new();
    public int Padding { get; protected set; }

    public BspLump(byte[] bytes) {
        Parse(bytes);
    }

    public virtual byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        foreach (var entry in Entries) {
            writer.Write(entry.ToBytes());
        }

        // Adding padding to be divisible by 4
        // TODO: this is bad. If ToBytes is never called, Padding is not initialized
        Padding = (int)(4 - writer.BaseStream.Length % 4) % 4;

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