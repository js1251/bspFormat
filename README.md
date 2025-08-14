# bspFormat
C# library for parsing valve bsp file format. Supports LZMA compressed bsps.

## Installation
1. Go to https://www.7-zip.org/sdk.html and download the latest 7zip sdk
2. Extract ONLY the "Cs" folder into "shared_bspFormat/"
Navigate to "shared_bspFormat/7zip/Compress/" and delete the folder "LzmaAlone"
3. Done!

## Usage

### Loading a bsp and accessing basic information
```csharp
// Loading a bsp
var bspFileInfo = new FileInfo("gm_construct.bsp");
var bsp = new Bsp(bspFileInfo);

// Bsp Header info
var bspHeader = bsp.BspHeader;
var bspIdent = bspHeader.Ident; // always 'VBSP' for valid bsp files
var bspVersion = bspHeader.Version;
var revision = bspHeader.MapRevision;

// Bsp Dictionary info (lump offsets, lenghts, ...)
var bspDictionary = bspHeader.BspLumpDictionary;
var entityLumpHeader = bspDictionary.GetLumpHeaderOfLumpWithId(Lump00_Entities.ID);

var entityLumpOffset = entityLumpHeader.Offset;
var entityLumpLength = entityLumpHeader.Length;
```

### Finding lumps in order
Lumps are not stored in order of their IDs (eg. the entity lump with ID 0 is not the first lump stored.) You can get the lumps in order of their appearance using `bspDictionary.GetLumpIdOfLumpAtPosition(int position)`.

```csharp
// print all lump IDs in order of appearance
for (var i = 0; i < Bsp.NUMBER_OF_LUMPS; i++) {
    var lumpId = bspDictionary.GetLumpIdOfLumpAtPosition(i);
    
    Console.WriteLine($"Lump ID at position {i}: {lumpId}");
}
```

### Manipulating lump data
```csharp
// Getting individual Lumps
var entityLump = bsp.Lumps[Lump00_Entities.ID] as Lump00_Entities;

// usage example:
foreach (var entry in entityLump.Entries) {
    if (entry is not Lump00_EntitiesEntry entityEntry) {
        throw new Exception("...");
    }

    for (var i = 0; i < entityEntry.Keys.Count; i++) {
        var key = entityEntry.Keys[i];
        var value = entityEntry.Values[i];
        
        // ...
    }
}
```

###  Saving bsp after changes
⚠️ Its important to call `bsp.RegenerateHeader()` before saving the bsp to adjust header values such as offsets and lengths to the actual lump contents.
```csharp
bsp.RegenerateHeader();
File.WriteAllBytes(bspFileInfo.FullName, bsp.ToBytes());
```

## Limitations
- Only bsp versions 19 and 20 are fully supported and tested.
- Not all lumps are fully supported. The full list of supported lumps can be found in [Bsp.cs](./shared_bspFormat/Bsp.cs). Remaining lumps will only allow byte-level interaction and do not offer parsing of the data or an interface to interact with it.