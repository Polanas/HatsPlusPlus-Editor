using OpenTK.Windowing.GraphicsLibraryFramework;

namespace HatsPlusPlusEditor;

public static class Mouse
{
    public static Vector2 ScreenPosition => _window.MousePosition;
    public static Vector2 IngamePosition { get; set; }
    public static Vector2 Velocity => _window.MouseState.Delta;
    public static MouseState State => _window.MouseState;

    private static Window _window = null!;

    public static void Init(Window window) => _window = window;

    public static bool Down(MouseButton button) => State.IsButtonDown(button);

    public static bool Pressed(MouseButton button) => !State.WasButtonDown(button) && State.IsButtonDown(button);

    public static bool Released(MouseButton button) => State.WasButtonDown(button) && !State.IsButtonDown(button);

    public static bool Any() => State.IsAnyButtonDown;
}
