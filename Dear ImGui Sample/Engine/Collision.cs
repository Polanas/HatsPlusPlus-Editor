using System.Runtime.CompilerServices;

namespace HatsPlusPlusEditor;

public static class Collision
{

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Point(Vector2 point, Vector2 position, Vector2 size)
    {
        float left = position.X;
        float right = position.X + size.X;
        float top = position.Y;
        float bottom = position.Y + size.Y;

        return point.X >= left && point.X <= right && point.Y >= top && point.Y <= bottom;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static bool Rectangle(Vector2 position1, Vector2 size1, Vector2 position2, Vector2 size2)
    {
        if (Math.Abs(position1.X - position2.X) > size1.X / 2 + size2.X / 2) return false;
        if (Math.Abs(position1.Y - position2.Y) > size1.Y / 2 + size2.Y / 2) return false;

        return true;
    }
}