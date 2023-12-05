namespace HatsPlusPlusEditor;

public static class MathExtensions
{
    public static Vector2 ToOpenTK(this SystemVector2 systemVector2) =>
        new(systemVector2.X, systemVector2.Y);

    public static Vector4 ToOpenTK(this SystemVector4 systemVector4) =>
       new(systemVector4.X, systemVector4.Y, systemVector4.Z, systemVector4.W);

    public static SystemVector2 ToSystem(this Vector2 vector2) =>
      new(vector2.X, vector2.Y);

    public static SystemVector2 ToSystem(this Vector2i vector2i) =>
     new(vector2i.X, vector2i.Y);

    public static SystemVector4 ToSystem(this Vector4 vector4) =>
  new(vector4.X, vector4.Y, vector4.Z, vector4.W);
}