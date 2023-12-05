namespace HatsPlusPlusEditor;

public enum HatType
{
    WereableHat,
    Wings,
    ExtraHat,
    FlyingPet,
    WalkingPet,
    Room,
    Preview
}

public enum LinkQuackAnimationsState
{
    FrameSaved,
    FrameInverted
}

public abstract class HatElement
{
    public HatType type;
    public Vector2i frameSize;
    public Texture texture = null!;
    public Vector2i hatAreaSize;
}

public abstract class PetHat : HatElement
{
    public int? distance;
    public bool noFlip;
    public List<HatAnimation> animations = new();
    public bool isBigHat;
    public LinkQuackAnimationsState? quackLinkState;
}

public class WalkingPetHat : PetHat
{
}

public class FlyingPetHat : PetHat
{
    public bool changesAngle;
    public int? speed;
}

public class ExtraHat : HatElement
{
}

public class RoomHat : HatElement
{
}

public class PreviewHat : HatElement
{
    public PreviewHat(Texture texture)
    {
        base.texture = texture;
    }
}

public class WingsHat : HatElement
{
    public Vector2i generalOffset;
    public Vector2i crouchOffset;
    public Vector2i ragdollOffset;
    public Vector2i slideOffset;
    public bool generateAnimations = true;
    public int? autoGlidingFrame;
    public int? autoIdleFrame;
    public int? autoAnimationsSpeed;
    public bool changeAnimationEveryLevel;
    public Vector2i netOffset;
    public bool isBigHat;//TODO: add this to ui
}

public class WereableHat : HatElement
{
    public bool isStrappedOn;
    public bool isBigHat;
    public List<HatAnimation> animations = new();
    public LinkQuackAnimationsState? quackLinkState;
    public int? onSpawnAnimation;
}

public class Hat
{
    public PreviewHat? previewHat = null!;
    public WereableHat? wereableHat = null!;
    public List<PetHat> pets = new();
    public RoomHat? roomHat;
    public WingsHat? wingsHat;
    public ExtraHat? extraHat;
    public string? path;

    public Hat(string? path = null!)
    {
        this.path = path;
    }
}
