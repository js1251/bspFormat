using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using shared_bspFormat.Lumps;

namespace shared_bspFormat;

public static class LumpMap {
    private static readonly Dictionary<int, Type> _map = new() {
        /* 00 */ { Lump00_Entities.ID, typeof(Lump00_Entities) },
        /* 01 */
        /* 02 */ { Lump02_TexData.ID, typeof(Lump02_TexData) },
        /* 03 */
        /* 04 */
        /* 05 */
        /* 06 */ { Lump06_TexInfo.ID, typeof(Lump06_TexInfo) },
        /* 07 */ { Lump07_Faces.ID, typeof(Lump07_Faces) },
        /* 08 */
        /* 09 */
        /* 10 */
        /* 11 */
        /* 12 */
        /* 13 */
        /* 14 */
        /* 15 */
        /* 16 */
        /* 17 */
        /* 18 */ { Lump18_Brushes.ID, typeof(Lump18_Brushes) },
        /* 19 */ { Lump19_BrushSides.ID, typeof(Lump19_BrushSides) },
        /* 20 */
        /* 21 */
        /* 22 */
        /* 23 */
        /* 24 */
        /* 25 */
        /* 26 */
        /* 27 */ { Lump27_OriginalFaces.ID, typeof(Lump27_OriginalFaces) },
        /* 28 */
        /* 29 */
        /* 30 */
        /* 31 */
        /* 32 */
        /* 33 */
        /* 34 */
        /* 35 */ { Lump35_GameLump.ID, typeof(Lump35_GameLump) },
        /* 36 */
        /* 37 */
        /* 38 */
        /* 39 */
        /* 40 */ // { Lump40_Pakfile.ID, typeof(Lump40_Pakfile) },
        /* 41 */
        /* 42 */ { Lump42_Cubemaps.ID, typeof(Lump42_Cubemaps) },
        /* 43 */ { Lump43_TexDataStringData.ID, typeof(Lump43_TexDataStringData) },
        /* 44 */ { Lump44_TexDataStringTable.ID, typeof(Lump44_TexDataStringTable) },
        /* 45 */
        /* 46 */
        /* 47 */
        /* 48 */
        /* 49 */
        /* 50 */
        /* 51 */
        /* 52 */
        /* 53 */
        /* 54 */
        /* 55 */
        /* 56 */
        /* 57 */
        /* 58 */
        /* 59 */
        /* 60 */
        /* 61 */
        /* 62 */
        /* 63 */
    };

    public static BspLump GetInstance(int index, byte[] bytes) {
        if (!_map.TryGetValue(index, out var type)) {
            return new DefaultBspLump(bytes);
        }

        if (Activator.CreateInstance(type, bytes) is not BspLump instance) {
            throw new Exception($"Failed to create instance of {type.Name}");
        }

        return instance;
    }
}

public class Bsp {
    public const int NUMBER_OF_LUMPS = 64;
    public BspHeader BspHeader { get; }
    public BspLump[] Lumps { get; }
    public FileSystemInfo Info { get; }

    public Bsp(FileSystemInfo fileInfo) {
        Info = fileInfo;

        var reader = new BinaryReader(new MemoryStream(File.ReadAllBytes(fileInfo.FullName)));

        BspHeader = new BspHeader(reader);
        Lumps = CreateLumps(reader);

        reader.Close();
    }

    public byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(BspHeader.ToBytes());

        for (var i = 0; i < NUMBER_OF_LUMPS; i++) {
            var lumpId = BspHeader.BspLumpDictionary.GetLumpIdOfLumpAtPosition(i);
            writer.Write(Lumps[lumpId].ToBytes());
            writer.Write(new byte[Lumps[lumpId].Padding]);
        }

        writer.Close();
        return stream.ToArray();
    }

    public void RegenerateHeader() {
        var offset = 0;
        var length = BspHeader.ToBytes().Length;
        var padding = 0;

        for (var i = 0; i < NUMBER_OF_LUMPS; i++) {
            var currentLumpId = BspHeader.BspLumpDictionary.GetLumpIdOfLumpAtPosition(i);
            var currentLumpHeader = BspHeader.BspLumpDictionary.GetLumpHeaderOfLumpWithId(currentLumpId);
            var currentLump = Lumps[currentLumpId];

            var currentLength = currentLump.ToBytes().Length;

            if (currentLength is not 0) {
                offset += length + padding;
                padding = 0;
            }

            var currentOffset = offset;

            // gamelump has offsets relative to entire bsp
            if (currentLumpId is Lump35_GameLump.ID) {
                if (currentLump is not Lump35_GameLump gameLump) {
                    throw new InvalidCastException("Could not cast to Lump35_GameLump!");
                }

                if (gameLump.Entries[0] is not GameLumpEntry gameLumpEntry) {
                    throw new InvalidCastException("Could not convert to Lump35_GameLumpInstance");
                }

                // int for gamelumpcount + header length for each gamelump instance
                var gameLumpHeaderLength = sizeof(int) + gameLumpEntry.GameLumps.Sum(instance => instance.ToBytes().Length);
                var instanceLength = 0;
                foreach (var instance in gameLumpEntry.GameLumps) {
                    instance.FileOffset = offset + gameLumpHeaderLength + instanceLength;
                    instanceLength = instance.FileLength;
                }
            }

            currentLumpHeader.Offset = currentOffset;
            currentLumpHeader.Length = currentLength;

            if (currentLength is not 0) {
                padding = currentLump.Padding;
                length = currentLength;
            }
        }
    }

    private BspLump[] CreateLumps(BinaryReader reader) {
        var lumps = new BspLump[NUMBER_OF_LUMPS];
        for (var i = 0; i < NUMBER_OF_LUMPS; i++) {
            var currentLumpId = BspHeader.BspLumpDictionary.GetLumpIdOfLumpAtPosition(i);
            var currentLumpHeader = BspHeader.BspLumpDictionary.GetLumpHeaderOfLumpWithId(currentLumpId);

            reader.BaseStream.Position = currentLumpHeader.Offset;
            lumps[currentLumpId] = LumpMap.GetInstance(currentLumpHeader.LumpNumber, reader.ReadBytes(currentLumpHeader.Length));
        }

        return lumps;
    }
}