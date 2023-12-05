namespace HatsPlusPlusEditor;

public struct Color : IEquatable<Color>
{
    public byte R, G, B, A;

    public Color(int R, int G, int B, int A)
    {
        this.R = (byte)R;
        this.B = (byte)B;
        this.G = (byte)G;
        this.A = (byte)A;
    }

    public Color(byte R, byte G, byte B, byte A)
    {
        this.R = R;
        this.B = B; 
        this.A = A;
    }

    public bool IsEmpty() => R == G && G == B && B == A && A == 0;

    public bool Equals(Color other)
    {
        return other.R == R && other.B == B && other.G == G && other.A == A;
    }
}
