using shared_bspFormat.Lumps;

namespace shared_bspFormat;

public static class LumpMap {
    private static readonly Dictionary<int, Type> _map = new() {
        /* 00 */ { EntityBspLump.ID, typeof(EntityBspLump) },
        /* 01 */
        /* 02 */
        /* 03 */
        /* 04 */
        /* 05 */
        /* 06 */ { TextInfoBspLump.ID, typeof(TextInfoBspLump) },
        /* 07 */ { FacesBspLump.ID, typeof(FacesBspLump) },
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
        /* 18 */ { BrushBspLump.ID, typeof(BrushBspLump) },
        /* 19 */ { BrushSideBspLump.ID, typeof(BrushSideBspLump) },
        /* 20 */
        /* 21 */
        /* 22 */
        /* 23 */
        /* 24 */
        /* 25 */
        /* 26 */
        /* 27 */ { OriginalFacesBspLump.ID, typeof(OriginalFacesBspLump) },
        /* 28 */
        /* 29 */
        /* 30 */
        /* 31 */
        /* 32 */
        /* 33 */
        /* 34 */
        /* 35 */ { GameLump.ID, typeof(GameLump) },
        /* 36 */
        /* 37 */
        /* 38 */
        /* 39 */
        /* 40 */ // { PakfileBspLump.ID, typeof(PakfileBspLump) },
        /* 41 */
        /* 42 */ { CubemapBspLump.ID, typeof(CubemapBspLump) },
        /* 43 */ { TextureStringDataBspLump.ID, typeof(TextureStringDataBspLump) },
        /* 44 */
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
            if (currentLumpId is GameLump.ID) {
                if (currentLump is not GameLump gameLump) {
                    throw new InvalidCastException("Could not cast to GameLump!");
                }

                if (gameLump.Entries[0] is not GameLumpEntry gameLumpEntry) {
                    throw new InvalidCastException("Could not convert to GameLumpInstance");
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