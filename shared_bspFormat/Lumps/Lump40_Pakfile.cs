using System;
using System.IO;
using System.IO.Compression;

namespace shared_bspFormat.Lumps;

public sealed class Lump40_PakfileEntry : LumpEntry {
    public string FullName { get; set; }
    public byte[] Data { get; set; }

    public Lump40_PakfileEntry(ZipArchiveEntry entry) {
        FullName = entry.FullName;

        var readStream = entry.Open();
        var writeStream = new MemoryStream();
        readStream.CopyTo(writeStream);
        readStream.Close();
        writeStream.Close();

        Data = writeStream.ToArray();
    }

    public override byte[] ToBytes() {
        throw new InvalidOperationException();
    }
}

public sealed class Lump40_Pakfile : BspLump {
    public const int ID = 40;

    private ZipArchive _archive;
    private int _currentReadIndex;

    public Lump40_Pakfile(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        if (_archive is null) {
            _archive = new ZipArchive(new MemoryStream(reader.ReadBytes((int)(reader.BaseStream.Length - reader.BaseStream.Position))));
        }

        if (_currentReadIndex >= _archive.Entries.Count) {
            return null;
        }

        var entry = _archive.Entries[_currentReadIndex];
        _currentReadIndex++;

        return new Lump40_PakfileEntry(entry);
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();

        using (var archive = new ZipArchive(stream, ZipArchiveMode.Create, true)) {
            foreach (var entry in Entries) {
                if (entry is not Lump40_PakfileEntry pakfileEntry) {
                    throw new Exception();
                }

                var zipEntry = archive.CreateEntry(pakfileEntry.FullName, CompressionLevel.NoCompression);
                using var entryStream = zipEntry.Open();
                entryStream.Write(pakfileEntry.Data, 0, pakfileEntry.Data.Length);
            }
        }

        return stream.ToArray();
    }
}