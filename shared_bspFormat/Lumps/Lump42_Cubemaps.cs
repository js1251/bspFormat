﻿using System.IO;

namespace shared_bspFormat.Lumps;

public sealed class Lump42_CubemapsEntry : LumpEntry {
    // struct dcubemapsample_t
    // {
    //     int origin[3];    // position of light snapped to the nearest integer
    //     int size;         // resolution of cubemap, 0 - default
    // };

    public int[] Origin { get; set; } = new int[3];
    public int Size { get; set; }

    public Lump42_CubemapsEntry(BinaryReader reader) {
        for (var i = 0; i < Origin.Length; i++) {
            Origin[i] = reader.ReadInt32();
        }

        Size = reader.ReadInt32();
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        foreach (var coordinate in Origin) {
            writer.Write(coordinate);
        }

        writer.Write(Size);

        writer.Close();
        return stream.ToArray();
    }
}

public sealed class Lump42_Cubemaps : BspLump {
    public const int ID = 42;
    public Lump42_Cubemaps(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        var bytesToRead = (int)(reader.BaseStream.Length - reader.BaseStream.Position);
        return bytesToRead is 0 ? null : new Lump42_CubemapsEntry(reader);
    }
}