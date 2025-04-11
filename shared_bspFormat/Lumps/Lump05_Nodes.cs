using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump05_NodeEntry : LumpEntry {
    //struct dnode_t
    //{
    //    int planenum;              // index into plane array
    //    int children[2];           // negative numbers are -(leafs + 1), not nodes
    //    short mins[3];             // for frustum culling
    //    short maxs[3];
    //    unsigned short firstface;  // index into face array
    //    unsigned short numfaces;   // counting both sides
    //    short area;                // If all leaves below this node are in the same area, then
    //                               // this is the area index. If not, this is -1.
    //    short paddding;            // pad to 32 bytes length
    //};

    public int PlaneNum { get; set; }
    public int[] Children { get; set; } = new int[2];
    public short[] Mins { get; set; } = new short[3];
    public short[] Maxs { get; set; } = new short[3];
    public ushort FirstFace { get; set; }
    public ushort NumFaces { get; set; }
    public short Area { get; set; }
    public short Padding { get; set; } // pad to 32 bytes length

    public Lump05_NodeEntry(BinaryReader reader) {
        PlaneNum = reader.ReadInt32();
        
        for (int i = 0; i < 2; i++) {
            Children[i] = reader.ReadInt32();
        }
        for (int i = 0; i < 3; i++) {
            Mins[i] = reader.ReadInt16();
        }
        for (int i = 0; i < 3; i++) {
            Maxs[i] = reader.ReadInt16();
        }

        FirstFace = reader.ReadUInt16();
        NumFaces = reader.ReadUInt16();
        Area = reader.ReadInt16();
        Padding = reader.ReadInt16();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(PlaneNum);

        for (int i = 0; i < 2; i++) {
            writer.Write(Children[i]);
        }
        for (int i = 0; i < 3; i++) {
            writer.Write(Mins[i]);
        }
        for (int i = 0; i < 3; i++) {
            writer.Write(Maxs[i]);
        }

        writer.Write(FirstFace);
        writer.Write(NumFaces);
        writer.Write(Area);
        writer.Write(Padding);

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class Lump05_Nodes : BspLump {
    public const int ID = 5;
    public Lump05_Nodes(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump05_NodeEntry(reader);
    }
}