using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HatsPlusPlusEditor;

public static class Keyboard
{ 
    public static KeyboardState State => _window.KeyboardState;

    private static Window _window = null!;

    public static void Init(Window window) => _window = window;

    public static bool Pressed(Keys key) => State.IsKeyPressed(key);

    public static bool Released(Keys key) => State.IsKeyReleased(key);

    public static bool DownAny() => State.IsAnyKeyDown;

    public static bool Down(Keys key) => State.IsKeyDown(key);
}