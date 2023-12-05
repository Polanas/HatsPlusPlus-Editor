using OpenTK.Graphics.OpenGL4;
using StbImageSharp;
using StbImageWriteSharp;
using System.IO;
using System.Runtime.CompilerServices;

namespace HatsPlusPlusEditor;

public abstract class AbstractTexture
{
    public Vector2i Size { get; protected set; }
    public int Width { get; protected set; }
    public int Height { get; protected set; }
    public string Name { get; protected set; } = null!;
}

public class Texture : AbstractTexture
{
    public int Handle { get; private set; }
    public string Path { get; private set; }

    public Texture(int handle, int width, int height, string name, string path)
    {
        Path = path;
        Handle = handle;
        Name = name;
        Width = width;
        Height = height;
        Size = new Vector2i(width, height);
    }

    public static implicit operator int(Texture texture) => texture.Handle;

    public void Use() => GL.BindTexture(TextureTarget.Texture2D, Handle);

    public byte[] GetData(PixelFormat pixelFormat = PixelFormat.Rgba, PixelType pixelType = PixelType.UnsignedByte)
    {
        byte[] data = new byte[Width * Height * 4];

        Use();
        GL.GetTexImage(TextureTarget.Texture2D, 0, pixelFormat, pixelType, data);

        return data;
    }

    public static Texture Load(string path)
    {
        ImageResult image;

#if DEBUG
        if (!File.Exists(path))
            throw new Exception($"Texture with path {path} could not be found.");

        try
        {
            using var stream = File.OpenRead(path);
            image = ImageResult.FromStream(stream, StbImageSharp.ColorComponents.RedGreenBlueAlpha);
        }
        catch
        {
            throw new FileNotFoundException($"Texture with path {path} could not be loaded");
        }
#else
        using (var stream = File.OpenRead(path))
            image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

#endif

        var name = System.IO.Path.GetFileNameWithoutExtension(path);
        var handle = GL.GenTexture();

        GL.ActiveTexture(TextureUnit.Texture0);
        GL.BindTexture(TextureTarget.Texture2D, handle);

        GL.TexImage2D(
            TextureTarget.Texture2D,
            0,
            PixelInternalFormat.Rgba,
            image.Width,
            image.Height,
            0,
            PixelFormat.Rgba,
            PixelType.UnsignedByte,
            image.Data);
        // GL.GenerateMipmap(GenerateMipmapTarget.Texture2D);

        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);

        GL.TextureParameter(handle, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
        GL.TextureParameter(handle, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);

        return new Texture(handle, image.Width, image.Height, name, path);
    }

    public void SaveRGBA(string path)
    {
        int dataLength = Width * Height * 4;
        byte[] data = new byte[dataLength];
        GL.GetTextureImage(Handle, 0, PixelFormat.Rgba, PixelType.UnsignedByte, dataLength, data);

        using FileStream fs = new(path, FileMode.OpenOrCreate);
        var writer = new ImageWriter();
        writer.WritePng(data, Width, Height, StbImageWriteSharp.ColorComponents.RedGreenBlueAlpha, fs);

        Path = path;
    }

    public void Delete()
    {
        GL.DeleteTexture(Handle);
    }
}
