namespace HatsPlusPlusEditor;

public enum MetapixelType
{
    StrappedOn,
    IsBigHat,
    FrameSize,
    AnimationType,
    AnimationDelay,
    AnimationLoop,
    AnimationFrame,
    AnimationFramePeriod,
    QuackAnimationsLinked,
    WingsGeneralOffset,
    WingsCrouchOffset,
    WingsRagdollOffset,
    WingsSlideOffset,
    GenerateWingsAnimations,
    PetChangeAngle,
    PetDistance,
    PetNoFlip,
    WingsAutoGlideFrame,
    WingsAutoIdleFrame,
    WingsAutoAnimationsSpeed,
    ChangeAnimationsEveryLevel,
    PetSpeed,
    WingsNetOffset,
    OnSpawnAnimation
}

public struct Metapixel
{
    public MetapixelType Type => (MetapixelType)R;
    public int R;
    public int G;
    public int B;

    public Metapixel(int r, int g, int b)
    {
        R = r;
        G = g;
        B = b;
    }

}