using OpenTK.Graphics.OpenGL4;

namespace HatsPlusPlusEditor;

public class StaticFBO
{
    public static FrameBuffer FBO { get; private set; } = null!;

    public static void Init()
    {
        var FBO = new FrameBuffer(DrawBuffersEnum.ColorAttachment0, 0);
        FBO.UseDefault();

        StaticFBO.FBO = FBO;
    }
}
