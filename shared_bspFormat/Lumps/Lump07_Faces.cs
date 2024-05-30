using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump07_FacesEntry : LumpEntry {
    //struct dface_t
    //{
    //    unsigned short planenum;            // the plane number
    //    byte side;                          // faces opposite to the node's plane direction
    //    byte onNode;                        // 1 of on node, 0 if in leaf
    //    int firstedge;                      // index into surfedges
    //    short numedges;                     // number of surfedges
    //    short texinfo;                      // texture info
    //    short dispinfo;                     // displacement info
    //    short surfaceFogVolumeID;           // ?
    //    byte styles[4];                     // switchable lighting info
    //    int lightofs;                       // offset into lightmap lump
    //    float area;                         // face area in units^2
    //    int LightmapTextureMinsInLuxels[2]; // texture lighting info
    //    int LightmapTextureSizeInLuxels[2]; // texture lighting info
    //    int origFace;                       // original face this was split from
    //    unsigned short numPrims;            // primitives
    //    unsigned short firstPrimID;         // ?
    //    unsigned int smoothingGroups;       // lightmap smoothing group
    //};

    public ushort PlaneNum { get; set; }
    public byte Side { get; set; }
    public byte OnNode { get; set; }
    public int FirstEdge { get; set; }
    public ushort NumEdges { get; set; }
    public ushort TexInfo { get; set; }
    public ushort DispInfo { get; set; }
    public ushort SurfaceFogVolumeID { get; set; }
    public byte[] Styles { get; set; } = new byte[4];
    public int LightOfs { get; set; }
    public float Area { get; set; }
    public int[] LightmapTextureMinsInLuxels { get; set; } = new int[2];
    public int[] LightmapTextureSizeInLuxels { get; set; } = new int[2];
    public int OrigFace { get; set; }
    public ushort NumPrims { get; set; }
    public ushort FirstPrimID { get; set; }
    public uint SmoothingGroups { get; set; }

    public Lump07_FacesEntry(BinaryReader reader) {
        PlaneNum = reader.ReadUInt16();
        Side = reader.ReadByte();
        OnNode = reader.ReadByte();
        FirstEdge = reader.ReadInt32();
        NumEdges = reader.ReadUInt16();
        TexInfo = reader.ReadUInt16();
        DispInfo = reader.ReadUInt16();
        SurfaceFogVolumeID = reader.ReadUInt16();

        for (var i = 0; i < Styles.Length; i++) {
            Styles[i] = reader.ReadByte();
        }

        LightOfs = reader.ReadInt32();
        Area = reader.ReadSingle();

        for (var i = 0; i < LightmapTextureMinsInLuxels.Length; i++) {
            LightmapTextureMinsInLuxels[i] = reader.ReadInt32();
        }

        for (var i = 0; i < LightmapTextureSizeInLuxels.Length; i++) {
            LightmapTextureSizeInLuxels[i] = reader.ReadInt32();
        }

        OrigFace = reader.ReadInt32();
        NumPrims = reader.ReadUInt16();
        FirstPrimID = reader.ReadUInt16();
        SmoothingGroups = reader.ReadUInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(PlaneNum);
        writer.Write(Side);
        writer.Write(OnNode);
        writer.Write(FirstEdge);
        writer.Write(NumEdges);
        writer.Write(TexInfo);
        writer.Write(DispInfo);
        writer.Write(SurfaceFogVolumeID);

        foreach (var style in Styles) {
            writer.Write(style);
        }

        writer.Write(LightOfs);
        writer.Write(Area);

        foreach (var luxel in LightmapTextureMinsInLuxels) {
            writer.Write(luxel);
        }

        foreach (var luxel in LightmapTextureSizeInLuxels) {
            writer.Write(luxel);
        }

        writer.Write(OrigFace);
        writer.Write(NumPrims);
        writer.Write(FirstPrimID);
        writer.Write(SmoothingGroups);

        writer.Close();

        return stream.ToArray();
    }
}

public class Lump07_Faces : BspLump {
    public const int ID = 7;
    public Lump07_Faces(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump07_FacesEntry(reader);
    }
}