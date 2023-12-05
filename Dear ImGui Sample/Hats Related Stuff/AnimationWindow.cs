using ImGuiNET;
using static ImGuiNET.ImGui;

namespace HatsPlusPlusEditor;

public class AnimationWindow
{
    public HatAnimation? CurrentAnimation { get; set; }
    public int CurrentFrame { get; set; }
    public HatElement? CurrentHatElement { get; set; }

    private int _frameTime;
    private Texture _renderTexture = null!;
    private HashSet<HatType> _supportedHatElements = new()
    {
        HatType.FlyingPet,
        HatType.WalkingPet,
        HatType.WereableHat,
        HatType.Wings
    };

    public void Init()
    {

    }

    public void TrySetHatElement(HatElement? element)
    {
        if (element == null || !_supportedHatElements.Contains(element.type))
        {
            CurrentHatElement = null;
            return;
        }

        CurrentHatElement = element;
        _renderTexture = element.texture;
    }

    public void SetAnimation(HatAnimation animation)
    {
        CurrentAnimation = animation;
        _frameTime = 0;
        CurrentFrame = 0;
    }

    public void Update()
    {
        if (CurrentHatElement == null)
            return;
        
        if (Begin("Animations Player"))
        {
            Image(_renderTexture, _renderTexture.Size.ToSystem());
            End();
        }
    }
}