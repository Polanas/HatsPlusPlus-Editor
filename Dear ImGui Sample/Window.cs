using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HatsPlusPlusEditor
{
    public class Window : GameWindow
    {
        private ImGuiController _controller = null!;
        private Editor _editor = null!;

        public Window() : base(GameWindowSettings.Default, new NativeWindowSettings(){ Size = new Vector2i(1600, 900), APIVersion = new Version(3, 3) })
        { }


        protected override void OnLoad()
        {
            base.OnLoad();

            var refreshRate = GetRefreshRate();

            Engine.Init(1f / refreshRate);
            StaticFBO.Init();
            Keyboard.Init(this);
            Mouse.Init(this);
            _editor = new();
            _editor.Init(this);

            Title += ": OpenGL Version: " + GL.GetString(StringName.Version);

            _controller = new ImGuiController(ClientSize.X, ClientSize.Y);
        }
        
        protected override void OnResize(ResizeEventArgs e)
        {
            base.OnResize(e);

            GL.Viewport(0, 0, ClientSize.X, ClientSize.Y);
            _controller.WindowResized(ClientSize.X, ClientSize.Y);
        }

        protected override void OnRenderFrame(FrameEventArgs e)
        {
            base.OnRenderFrame(e);

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit | ClearBufferMask.StencilBufferBit);

            _controller.Render();

            ImGuiController.CheckGLError("End of frame");

            SwapBuffers();
        }

        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            base.OnUpdateFrame(e);

            _controller.Update(this, (float)e.Time);
            _editor.Update();
        }

        protected override void OnTextInput(TextInputEventArgs e)
        {
            base.OnTextInput(e);
            
            
            _controller.PressChar((char)e.Unicode);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            
            _controller.MouseScroll(e.Offset);
        }

        private unsafe int GetRefreshRate()
        {
            var monitor = GLFW.GetPrimaryMonitor();
            var videoMode = GLFW.GetVideoMode(monitor);
            var monitorSize = new Vector2i(videoMode->Width, videoMode->Height);
            int refreshRate = videoMode->RefreshRate;

            return refreshRate;
        }
    }
}
