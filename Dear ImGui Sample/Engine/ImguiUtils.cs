using ImGuiNET;
using static ImGuiNET.ImGui;

namespace HatsPlusPlusEditor;

static class ImguiUtils
{
    public static void TextVerticallyAlignedWithButtons(string text)
    {
        var textSize = CalcTextSize("some text");
        var buttonSize = textSize + GetStyle().FramePadding * 2;
        var textYOffset = buttonSize.Y / 2 - textSize.Y / 2;
        SetCursorPosY(GetCursorPosY() + textYOffset);
        Text(text);
        SameLine();
        SetCursorPosY(GetCursorPosY() - textYOffset);
    }

    public static bool TextLinkVerticallyAlignedWithButtons(string text, bool changeColor = true)
    {
        var textSize = CalcTextSize("some text");
        var buttonSize = textSize + GetStyle().FramePadding * 2;
        var textYOffset = buttonSize.Y / 2 - textSize.Y / 2;
        SetCursorPosY(GetCursorPosY() + textYOffset);
        var result = TextLink(text, changeColor);
        SameLine();
        SetCursorPosY(GetCursorPosY() - textYOffset);

        return result;
    }

    public static bool TextLink(string text, bool changeColor = true)
    {
        bool collides = Collision.Point(Mouse.ScreenPosition, GetCursorPos().ToOpenTK(), CalcTextSize(text).ToOpenTK());
        var color = collides && changeColor ? GetStyle().Colors[(int)ImGuiCol.TabHovered].ToOpenTK() : new Vector4(255);

        TextColored(color.ToSystem(), text);

        return collides && Mouse.Pressed(MouseButton.Button1);
    }
}
