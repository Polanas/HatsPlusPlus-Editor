using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using StbImageWriteSharp;
using System.IO;
using System.Runtime.CompilerServices;

namespace HatsPlusPlusEditor;

public class MyBitmap
{
    public int Width { get; private set; }
    public int Height { get; private set; }
    public Texture? Texture { get; private set; }
    public byte[] Data { get; private set; }

    public static MyBitmap Create(Texture texture)
    {
        var data = new byte[texture.Size.X * texture.Size.Y * 4];
        ReadPixels(texture, ref data[0]);

        return new MyBitmap(texture, data);
    }

    public static MyBitmap Create(int width, int height)
    {
        var data = new byte[height * width * 4];
        return new MyBitmap(data, width, height);
    }

    private MyBitmap(Texture texture, byte[] data)
    {
        Data = data;
        Texture = texture;
        Width = Texture.Width;
        Height = texture.Height;
    }

    private MyBitmap(byte[] data, int width, int height)
    {
        Data = data;
        Width = width;
        Height = height;
    }

    public void Save(string path)
    {
        using FileStream fs = new(path, FileMode.OpenOrCreate);
        var writer = new ImageWriter();
        writer.WritePng(Data, Width, Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, fs);
    }

    public void Empty(Vector2i position, Vector2i size)
    {
        var originalPosition = position;
        for (; position.X < size.X + originalPosition.X; position.X++)
        {
            for (; position.Y < size.Y + originalPosition.Y; position.Y++)
                SetPixel(position, new Color());

            position.Y = 0;
        }
    }

    public unsafe void Draw(Texture texture, Vector2i position)
    {
        var data = stackalloc byte[texture.Width * texture.Height * 4];
        ReadPixels(texture, ref data[0]);

        for (; position.X < texture.Width; position.X++)
        {
            for (; position.Y < texture.Height; position.Y++)
            {
                var index = position.Y * texture.Width + position.X;
                var R = data[index * 4];
                var G = data[index * 4 + 1];
                var B = data[index * 4 + 2];
                var A = data[index * 4 + 3];
                SetPixel(position, new Color(A, R, G, B));
            }

            position.Y = 0;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public Color GetPixel(Vector2i position)
    {
        var index = Utils.GetIndex(position, Width);

        if (index * 4 >= Data.Length || index < 0)
            return new Color();

        return new Color(
            Data[index * 4 + 3],
            Data[index * 4],
            Data[index * 4 + 1],
            Data[index * 4 + 2]);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void SetPixel(Vector2i position, Color Color)
    {
        var index = Utils.GetIndex(position, Width);

        if (index * 4 >= Data.Length || index < 0)
            return;

        Data[index * 4] = Color.R;
        Data[index * 4 + 1] = Color.G;
        Data[index * 4 + 2] = Color.B;
        Data[index * 4 + 3] = Color.A;
    }

    private static void ReadPixels(Texture texture, ref byte data)
    {
        var FBO = StaticFBO.FBO;
        FBO.Use();
        FBO.UseTexture(texture, FramebufferAttachment.ColorAttachment0);
        GL.ReadPixels(0, 0, texture.Width, texture.Height, PixelFormat.Rgba, PixelType.UnsignedByte, ref data);
        FBO.UseDefault();
    }
}
