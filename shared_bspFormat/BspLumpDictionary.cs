using System;
using System.IO;

namespace shared_bspFormat;

public sealed class BspLumpDictionaryEntry {
    //struct lump_t
    //{
    //    int fileofs;      // offset into file (bytes)
    //    int filelen;      // length of lump (bytes)
    //    int version;      // lump format version
    //    char fourCC[4];   // lump ident code
    //};
    public int LumpNumber { get; }
    public int Offset { get; set; }
    public int Length { get; set; }
    public int Version { get; }
    public char[] FourCC { get; set; } = new char[4];

    public BspLumpDictionaryEntry(BinaryReader reader, int lumpNumber) {
        Offset = reader.ReadInt32();
        Length = reader.ReadInt32();
        Version = reader.ReadInt32();

        for (var i = 0; i < FourCC.Length; i++) {
            FourCC[i] = reader.ReadChar();
        }

        LumpNumber = lumpNumber;
    }

    public byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        writer.Write(Offset);
        writer.Write(Length);
        writer.Write(Version);

        foreach (var cc in FourCC) {
            writer.Write(cc);
        }

        writer.Close();

        return stream.ToArray();
    }

    public bool IsLZMACompressed() {
        return new string(FourCC) == "LZMA";
    }
}

public class BspLumpDictionary {
    private readonly BspLumpDictionaryEntry[] _dictionaryEntries = new BspLumpDictionaryEntry[Bsp.NUMBER_OF_LUMPS];
    private readonly int[] _lumpOrder = new int[Bsp.NUMBER_OF_LUMPS];

    public BspLumpDictionary(BinaryReader reader) {
        for (var i = 0; i < _dictionaryEntries.Length; i++) {
            _dictionaryEntries[i] = new BspLumpDictionaryEntry(reader, i);
            _lumpOrder[i] = i;
        }

        // sort lumpOrder based on offsets of _dictionaryEntries
        Array.Sort(_lumpOrder, (a, b) => {
            var headerA = _dictionaryEntries[a];
            var headerB = _dictionaryEntries[b];

            var result = headerA.Offset.CompareTo(headerB.Offset);

            return result is not 0 ? result : b.CompareTo(a);
        });
    }

    public BspLumpDictionaryEntry GetLumpHeaderOfLumpWithId(int lumpId) {
        return _dictionaryEntries[lumpId];
    }

    public int GetLumpIdOfLumpAtPosition(int position) {
        return _lumpOrder[position];
    }

    public byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        foreach (var entry in _dictionaryEntries) {
            writer.Write(entry.ToBytes());
        }

        writer.Close();
        return stream.ToArray();
    }
}