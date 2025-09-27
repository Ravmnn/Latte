using System;
using System.IO;
using System.Linq;
using System.Reflection;

using SFML.Graphics;
using SFML.Graphics.Glsl;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;


namespace Latte.Rendering;




[AttributeUsage(AttributeTargets.Property)]
public class UniformAttribute : Attribute;




public class Effect : Shader
{
    private PropertyInfo[]? _uniformProperties;




    [Uniform] public float Time { get; protected set; }
    [Uniform] public Vec2i WindowResolution { get; protected set; } = new Vec2i();
    [Uniform] public Vec2i TargetResolution { get; protected set; } = new Vec2i();
    [Uniform] public Vec2i MousePosition { get; protected set; } = new Vec2i();




    public Effect(string? fragmentShaderFilename, string? vertexShaderFilename = null)
        : base(vertexShaderFilename, null, fragmentShaderFilename)
    {}


    public Effect(Stream? fragmentShaderStream, Stream? vertexShaderStream = null)
        : base(vertexShaderStream, null, fragmentShaderStream)
    {}




    public virtual void UpdateUniforms(IRenderer renderer)
    {
        Time = (float)DeltaTime.FromStartSeconds;
        WindowResolution = (Vec2u)App.Window.Size;
        TargetResolution = (Vec2u)renderer.RenderTarget.Size;
        MousePosition = MouseInput.Position;

        PropertiesToUniforms();
    }




    private void PropertiesToUniforms()
    {
        InitializeUniformProperties();

        foreach (var uniformProperty in _uniformProperties!)
            PropertyToUniform(uniformProperty);
    }


    private void PropertyToUniform(PropertyInfo property)
    {
        var name = $"u{property.Name}";

        switch (property.GetValue(this))
        {
            case float value:
                SetUniform(name, value);
                break;

            case Vec2f value:
                SetUniform(name, new Vec2(value));
                break;

            case Vec2i value:
                SetUniform(name, new Vec2(value));
                break;

            case Vec2u value:
                SetUniform(name, new Vec2(value));
                break;

            case ColorRGBA value:
                SetUniform(name, value);
                break;

            case Texture value:
                SetUniform(name, value);
                break;
        }
    }




    private void InitializeUniformProperties()
    {
        if (_uniformProperties is not null)
            return;

        _uniformProperties = (from property in GetType().GetProperties()
            where property.HasAttribute<UniformAttribute>()
            select property).ToArray();
    }
}
