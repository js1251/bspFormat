namespace shared_bspFormat.Lumps;

public sealed class GameLumpInstance : LumpEntry {
    /*
     * struct dgamelump_t
     * {
     *     int             id;        // gamelump ID
     *     unsigned short  flags;     // flags
     *     unsigned short  version;   // gamelump version
     *     int             fileofs;   // offset to this gamelump
     *     int             filelen;   // length
     * };
     */

    public int Id { get; set; }
    public ushort Flags { get; set; }
    public ushort Version { get; set; }
    public int FileOffset { get; set; }
    public int FileLength { get; set; }
    public byte[] Data { get; set; }

    public GameLumpInstance(BinaryReader reader) {
        Id = reader.ReadInt32();
        Flags = reader.ReadUInt16();
        Version = reader.ReadUInt16();
        FileOffset = reader.ReadInt32();
        FileLength = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(Id);
        writer.Write(Flags);
        writer.Write(Version);
        writer.Write(FileOffset);
        writer.Write(FileLength);

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class GameLumpEntry : LumpEntry {
    /*
     * struct dgamelumpheader_t
     * {
     *      int lumpCount;  // number of game lumps
     *      dgamelump_t gamelump[lumpCount];
     * };
     */

    public int LumpCount { get; set; }
    public GameLumpInstance[] GameLumps { get; set; }

    public GameLumpEntry(BinaryReader reader) {
        LumpCount = reader.ReadInt32();

        GameLumps = new GameLumpInstance[LumpCount];
        for (var i = 0; i < LumpCount; i++) {
            GameLumps[i] = new GameLumpInstance(reader);
        }

        // read the actual lump data
        for (var i = 0; i < LumpCount; i++) {
            GameLumps[i].Data = reader.ReadBytes(GameLumps[i].FileLength);
        }
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(LumpCount);

        foreach (var instance in GameLumps) {
            writer.Write(instance.ToBytes());
        }

        foreach (var instance in GameLumps) {
            writer.Write(instance.Data);
        }

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class GameLump : BspLump {
    public const int ID = 35;

    public GameLump(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new GameLumpEntry(reader);
    }
}