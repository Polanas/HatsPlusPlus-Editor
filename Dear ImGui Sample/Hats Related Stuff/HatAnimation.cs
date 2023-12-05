namespace HatsPlusPlusEditor;

public enum HatAnimationType
{
    OnDefault,
    OnPressQuack,
    OnReleaseQuack,
    OnStatic,
    OnApproach,
    OnDuckDeath,
    Flying,
    StartIdle,
    Gliding,
    StartGliding,
    Idle,
    OnRessurect
}

public class HatAnimation
{
    public HatAnimationType type;
    public int delay;
    public bool looping;
    public List<int> frames;
    public int newFrame;
    public int frameIndexToDelete = -1;
    public bool newFrameWasAdded;
    public Vector2i newRange;
    public bool wasOpen;
    private readonly static Dictionary<HatAnimationType, string> _animationNames = new()
    {
        { HatAnimationType.OnDefault, "On Default" },
        {HatAnimationType.OnRessurect, "On Ressurect" },
        {HatAnimationType.OnStatic, "On Static" },
        {HatAnimationType.OnPressQuack, "On Press Quack" },
        {HatAnimationType.OnReleaseQuack, "On Release Quack" },
        {HatAnimationType.Gliding, "Gliding" },
        {HatAnimationType.StartIdle, "Start Idle" },
        {HatAnimationType.StartGliding, "Start Gliding" },
        {HatAnimationType.Flying, "Flying" },
        {HatAnimationType.OnApproach, "On Approach" },
        {HatAnimationType.Idle, "Idle" },
        {HatAnimationType.OnDuckDeath, "On Duck Death" },
    };

    public string Name => _animationNames[type];

    public HatAnimation(HatAnimationType type, int delay, bool looping, List<int> frames)
    {
        this.type = type;
        this.delay = delay;
        this.looping = looping;
        this.frames = frames;
    }
}