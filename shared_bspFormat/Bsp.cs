using shared_bspFormat.Lumps;

namespace shared_bspFormat;

internal static class LumpMap {
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
        /* 27 */
        /* 28 */
        /* 29 */
        /* 30 */
        /* 31 */
        /* 32 */
        /* 33 */
        /* 34 */
        /* 35 */
        /* 36 */
        /* 37 */
        /* 38 */
        /* 39 */
        /* 40 */
        /* 41 */
        /* 42 */
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

    internal static BspLump GetInstance(int index, byte[] bytes) {
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
            var lumpNumber = BspHeader.BspLumpDictionary.GetLumpNumberOfLumpAtPosition(i);
            writer.Write(Lumps[lumpNumber].ToBytes());
        }

        writer.Close();
        return stream.ToArray();
    }

    private BspLump[] CreateLumps(BinaryReader reader) {
        var lumps = new BspLump[NUMBER_OF_LUMPS];
        for (var i = 0; i < NUMBER_OF_LUMPS; i++) {
            var currentLumpNumber = BspHeader.BspLumpDictionary.GetLumpNumberOfLumpAtPosition(i);
            var currentLumpHeader = BspHeader.BspLumpDictionary.GetLumpHeader(currentLumpNumber);

            // read next lump
            lumps[currentLumpNumber] = LumpMap.GetInstance(currentLumpHeader.LumpNumber, reader.ReadBytes(currentLumpHeader.Length));

            long difference;
            var currentReadOffset = reader.BaseStream.Position;
            if (i < NUMBER_OF_LUMPS - 1) {
                var nextLumpNumber = BspHeader.BspLumpDictionary.GetLumpNumberOfLumpAtPosition(i + 1);
                var nextLumpHeader = BspHeader.BspLumpDictionary.GetLumpHeader(nextLumpNumber);

                var nextLumpLength = nextLumpHeader.Length;
                // ignore empty lumps
                if (nextLumpLength is 0) {
                    continue;
                }

                difference = nextLumpHeader.Offset - currentReadOffset;
            } else {
                difference = reader.BaseStream.Length - reader.BaseStream.Position;
            }

            if (difference is not 0) {
                lumps[currentLumpNumber].Padding = reader.ReadBytes((int)difference);
            }
        }

        return lumps;
    }
}