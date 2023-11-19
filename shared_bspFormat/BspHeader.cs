namespace shared_bspFormat;

public class BspHeader {
    /* struct dheader_t
     * {
     *     int     ident;                  // BSP file identifier
     *     int     version;                // BSP file version
     *     lump_t  lumps[HEADER_LUMPS];    // lump directory array
     *     int     mapRevision;            // the map's revision (iteration, version) number
     * };
     */

    public int Ident { get; }
    public int Version { get; }
    public BspLumpDictionary BspLumpDictionary { get; }
    public int MapRevision;

    public BspHeader(BinaryReader reader) {
        Ident = reader.ReadInt32();
        Version = reader.ReadInt32();
        BspLumpDictionary = new BspLumpDictionary(reader);
        MapRevision = reader.ReadInt32();
    }

    public BspHeader(byte[] bytes) : this(new BinaryReader(new MemoryStream(bytes))) { }

    public byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(Ident);
        writer.Write(Version);
        writer.Write(BspLumpDictionary.ToBytes());
        writer.Write(MapRevision);

        writer.Close();

        return stream.ToArray();
    }
}