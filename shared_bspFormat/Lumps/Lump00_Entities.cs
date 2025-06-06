﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace shared_bspFormat.Lumps;

public sealed class Lump00_EntitiesEntry : LumpEntry {
    public List<string> Keys { get; } = new();
    public List<string> Values { get; } = new();
    public Lump00_EntitiesEntry() { }

    public Lump00_EntitiesEntry(BinaryReader reader) {
        reader.ReadChar(); // opening brace

        while (ReadKey(reader)) {
            ReadValue(reader);
        }

        RemoveWhitespace(reader);
    }

    public override byte[] ToBytes() {
        var result = "{\n";

        for (var i = 0; i < Keys.Count; i++) {
            result += $"\"{Keys[i]}\" \"{Values[i]}\"\n";
        }

        result += "}\n";

        return Encoding.ASCII.GetBytes(result);
    }

    private bool ReadKey(BinaryReader reader) {
        RemoveWhitespace(reader);

        var first = reader.ReadChar();
        if (first == '}') {
            return false;
        }

        var key = new StringBuilder();
        var c = reader.ReadChar();
        while (c != '"') {
            key.Append(c);
            c = reader.ReadChar();
        }

        Keys.Add(key.ToString());

        return true;
    }

    private void ReadValue(BinaryReader reader) {
        RemoveWhitespace(reader);
        var value = new StringBuilder();

        reader.ReadChar(); // '"' prefix

        var c = reader.ReadChar();
        while (c != '"') {
            value.Append(c);
            c = reader.ReadChar();
        }

        Values.Add(value.ToString());
    }

    private static void RemoveWhitespace(BinaryReader reader) {
        byte b;
        do {
            b = reader.ReadByte();
        } while (b == ' ' || b == '\t' || b == '\r' || b == '\n');

        reader.BaseStream.Seek(-1, SeekOrigin.Current);
    }
}

public sealed class Lump00_Entities : BspLump {
    public const int ID = 0;
    public Lump00_Entities(byte[] bytes) : base(bytes) { }

    protected override LumpEntry ProvideEntry(BinaryReader reader) {
        if (reader.BaseStream.Length is 0) {
            return null;
        }

        var nextChar = reader.ReadChar();
        if (nextChar is '\0') {
            return null;
        }

        reader.BaseStream.Seek(-1, SeekOrigin.Current);
        return new Lump00_EntitiesEntry(reader);
    }

    public override byte[] ToBytes() {
        var stream = new MemoryStream();
        var writer = new BinaryWriter(stream);

        foreach (var entry in Entries) {
            if (entry is not Lump00_EntitiesEntry entityEntry) {
                throw new Exception("Invalid lump type");
            }

            writer.Write(entityEntry.ToBytes());
        }

        // add null terminator
        writer.Write(new byte[1]);

        writer.Close();
        return stream.ToArray();
    }
}