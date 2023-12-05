namespace HatsPlusPlusEditor;

class Utils
{
    /// <summary>
    /// Returns and index of a 1d array, given 2d array index and width
    /// </summary>
    public static int GetIndex(Vector2i position, int width) =>
       position.Y * width + position.X;
}
