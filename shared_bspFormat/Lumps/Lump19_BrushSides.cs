﻿using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump19_BrushSidesEntry : LumpEntry {
    //struct <code>dbrushside_t</code>
    //{
    //    unsigned short planenum;     // facing out of the leaf
    //    short texinfo;      // texture info
    //    short dispinfo;     // displacement info
    //    short bevel;        // is the side a bevel plane?
    //};

    public ushort PlaneNum { get; set; }
    public short TexInfo { get; set; }
    public short DispInfo { get; set; }
    public short Bevel { get; set; }

    public Lump19_BrushSidesEntry(BinaryReader reader) {
        PlaneNum = reader.ReadUInt16();
        TexInfo = reader.ReadInt16();
        DispInfo = reader.ReadInt16();
        Bevel = reader.ReadInt16();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(PlaneNum);
        writer.Write(TexInfo);
        writer.Write(DispInfo);
        writer.Write(Bevel);

        writer.Close();

        return stream.ToArray();
    }
}

public sealed class Lump19_BrushSides : BspLump {
    public const int ID = 19;
    public Lump19_BrushSides(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump19_BrushSidesEntry(reader);
    }
}