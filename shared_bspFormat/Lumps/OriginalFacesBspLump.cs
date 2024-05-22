namespace shared_bspFormat.Lumps;

public sealed class OriginalFacesBspLump : FacesBspLump {
    public const int ID = 27; // TODO: Is this a problem?
    public OriginalFacesBspLump(byte[] bytes) : base(bytes) { }
}