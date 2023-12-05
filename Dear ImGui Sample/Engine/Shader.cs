using OpenTK.Graphics.OpenGL4;
using System.IO;

namespace HatsPlusPlusEditor;

public class Shader
{
    public int Handle { get; private set; }
    public string Name { get; private set; }
    public string VertPath { get; private set; }
    public string FragPath { get; private set; }

    private Dictionary<string, int> _uniformLocations = new();

    public Shader(int handle, Dictionary<string, int> uniformLocations, string name, string vertPath, string fragPath)
    {
        Handle = handle;
        Name = name;
        _uniformLocations = uniformLocations;
        VertPath = vertPath;
        FragPath = fragPath;
    }

    public static implicit operator int(Shader shader) => shader.Handle;

    public void Use() => GL.UseProgram(Handle);

    public void Reset(int handle, Dictionary<string, int> uniformLocations)
    {
        Handle = handle;
        _uniformLocations = uniformLocations;
    }

    public int GetUniformLocation(string name) => GL.GetUniformLocation(Handle, name);

    public void UnsetTexture(string name, TextureUnit unit)
    {
        SetValue(name, (int)unit - (int)TextureUnit.Texture0);
        GL.ActiveTexture(unit);
        GL.BindTexture(TextureTarget.Texture2D, 0);
    }

    public unsafe void SetValue(string name, Vector3i[][] value)
    {
        var unifromName = name + "[0][0]";
        if (!_uniformLocations.TryGetValue(unifromName, out var uniformLocation))
            return;

        fixed (Vector3i* p = value[0])
            GL.Uniform3(uniformLocation, value.Length, (int*)p);
    }


    public unsafe void SetValue(string name, Vector2[] value)
    {
        var unifromName = name + "[0]";
        if (!_uniformLocations.TryGetValue(unifromName, out var uniformLocation))
            return;

        fixed (Vector2* p = value)
            GL.Uniform2(uniformLocation, value.Length, (float*)p);
    }

    public unsafe void SetValue(string name, Span<Vector2> value)
    {
        var unifromName = name + "[0]";
        if (!_uniformLocations.TryGetValue(unifromName, out var uniformLocation))
            return;

        fixed (void* p = value)
            GL.Uniform2(uniformLocation, value.Length, ref ((float*)p)[0]);
    }

    public unsafe void SetValue(string name, Vector3[] value)
    {
        var unifromName = name + "[0]";
        if (!_uniformLocations.TryGetValue(unifromName, out var uniformLocation))
            return;

        fixed (Vector3* p = value)
            GL.Uniform3(uniformLocation, value.Length, (float*)p);
    }

    public unsafe void SetValue(string name, Vector3i[] value)
    {
        var unifromName = name + "[0]";
        if (!_uniformLocations.TryGetValue(unifromName, out var uniformLocation))
            return;

        fixed (Vector3i* p = value)
            GL.Uniform3(uniformLocation, value.Length, (int*)p);
    }

    public unsafe void SetValue(string name, Span<Vector3> value)
    {
        var unifromName = name + "[0]";
        if (!_uniformLocations.TryGetValue(unifromName, out var uniformLocation))
            return;

        fixed (void* p = value)
            GL.Uniform3(uniformLocation, value.Length, ref ((float*)p)[0]);
    }

    public void SetValue(string name, float[] value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform2(_uniformLocations[name + "[0]"], value.Length, value);
    }

    public void SetValue(string name, int value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform1(_uniformLocations[name], value);
    }

    public void SetValue(string name, bool value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform1(_uniformLocations[name], value ? 1 : 0);
    }

    public void SetValue(string name, float value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform1(_uniformLocations[name], value);
    }

    public void SetValue(string name, Vector2 value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform2(_uniformLocations[name], value);
    }

    public void SetValue(string name, Vector2i value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform2(_uniformLocations[name], value);
    }

    public void SetValue(string name, Vector3 value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform3(_uniformLocations[name], value);
    }

    public void SetValue(string name, Vector3i value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform3(_uniformLocations[name], value);
    }

    public void SetValue(string name, Vector4 value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.Uniform4(_uniformLocations[name], value);
    }

    public void SetValue(string name, Matrix4 value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.UniformMatrix4(_uniformLocations[name], true, ref value);
    }

    public void SetValue(string name, ref Matrix4 value)
    {
        if (!_uniformLocations.ContainsKey(name))
            return;

        GL.UniformMatrix4(_uniformLocations[name], true, ref value);
    }

    public static Shader Load(string path)
    {
        var vertPath = Path.Combine(path, "vert.glsl");
        var fragPath = Path.Combine(path, "frag.glsl");
        string vertSource = File.ReadAllText(vertPath);
        string fragSource = File.ReadAllText(fragPath);
        string name = Path.GetFileNameWithoutExtension(path);

        int vertShader, fragShader;
        int compileStatus, linkStatus;
        Dictionary<string, int> uniforms = new();

        vertShader = GL.CreateShader(ShaderType.VertexShader);
        GL.ShaderSource(vertShader, vertSource);
        compileStatus = Compile(vertShader);
        ProcessCompilation(vertShader, name, compileStatus);

        fragShader = GL.CreateShader(ShaderType.FragmentShader);
        GL.ShaderSource(fragShader, fragSource);
        compileStatus = Compile(fragShader);
        ProcessCompilation(fragShader, name, compileStatus);

        int handle = GL.CreateProgram();

        GL.AttachShader(handle, vertShader);
        GL.AttachShader(handle, fragShader);

        linkStatus = Link(handle);
        ProcessLinking(handle, name, linkStatus);

        GL.DetachShader(handle, fragShader);
        GL.DetachShader(handle, vertShader);
        GL.DeleteShader(fragShader);
        GL.DeleteShader(vertShader);

        GL.GetProgram(handle, GetProgramParameterName.ActiveUniforms, out var numberOfUniforms);

        for (int i = 0; i < numberOfUniforms; i++)
        {
            var key = GL.GetActiveUniform(handle, i, out _, out _);
            var location = GL.GetUniformLocation(handle, key);
            uniforms.Add(key, location);
        }

        return new Shader(handle, uniforms, name, vertPath, fragPath);
    }

    private static int Link(int handle)
    {
        GL.LinkProgram(handle);
        GL.GetProgram(handle, GetProgramParameterName.LinkStatus, out var linkStatus);

        return linkStatus;
    }

    private static void ProcessLinking(int handle, string name, int linkStatus)
    {
        if (linkStatus == (int)All.True)
            return;

        var infoLog = GL.GetProgramInfoLog(handle);
        throw new Exception($"Error occurred while linking shader {name}:\n{infoLog}");
    }

    private static int Compile(int handle)
    {
        GL.CompileShader(handle);
        GL.GetShader(handle, ShaderParameter.CompileStatus, out var compileStatus);

        return compileStatus;
    }

    private static void ProcessCompilation(int handle, string name, int compileStatus)
    {
        if (compileStatus == (int)All.True)
            return;

        var infoLog = GL.GetShaderInfoLog(handle);
        throw new Exception($"Error occurred while compiling shader {name}:\n{infoLog}");
    }
}