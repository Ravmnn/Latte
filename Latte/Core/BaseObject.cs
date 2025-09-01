using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Application.Elements.Properties;
using Latte.Exceptions.Element;


namespace Latte.Core;


public abstract class BaseObject : IUpdateable, IDrawable
{
    private int _priority;
    private bool _visible;
    private readonly List<Property> _properties;


    public abstract Transformable SfmlTransformable { get; }
    public abstract Drawable SfmlDrawable { get; }

    public IEnumerable<Property> Properties => _properties;

    public bool Initialized { get; private set; }

    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value)
                return;

            _visible = value;
            OnVisibilityChange();
        }
    }

    public bool CanDraw => Initialized && Visible;

    public int Priority
    {
        get => _priority;
        set
        {
            if (_priority == value)
                return;

            _priority = value;
            OnPriorityChange();
        }
    }

    protected int LastPriority { get; private set; }

    public AnimatableProperty<Vec2f> Position { get; }
    public AnimatableProperty<Vec2f> Origin { get; }
    public AnimatableProperty<Float> Rotation { get; }
    public AnimatableProperty<Vec2f> Scale { get; }

    public event EventHandler? SetupEvent;
    public event EventHandler? ConstantUpdateEvent;
    public event EventHandler? UpdateEvent;
    public event EventHandler? DrawEvent;

    public event EventHandler? VisibilityChangedEvent;
    public event EventHandler? PriorityChangedEvent;


    public BaseObject()
    {
        _properties = [];

        Visible = true;

        Position = new AnimatableProperty<Vec2f>(this, nameof(Position), new Vec2f());
        Origin = new AnimatableProperty<Vec2f>(this, nameof(Origin), new Vec2f());
        Rotation = new AnimatableProperty<Float>(this, nameof(Rotation), 0f);
        Scale = new AnimatableProperty<Vec2f>(this, nameof(Scale), new Vec2f(1f, 1f));
    }


    public virtual void Setup()
    {
        Initialized = true;

        SetupEvent?.Invoke(this, EventArgs.Empty);
    }


    // called at least one time each frame, independently of visibility. May be called
    // more than one time a frame
    public virtual void ConstantUpdate()
    {
        if (!Initialized)
            Setup();

        UpdateSfmlProperties();

        LastPriority = Priority;

        ConstantUpdateEvent?.Invoke(this, EventArgs.Empty);
    }


    protected virtual void UpdateSfmlProperties()
    {
        SfmlTransformable.Position = Position.Value;
        SfmlTransformable.Origin = Origin.Value;
        SfmlTransformable.Rotation = Rotation.Value;
        SfmlTransformable.Scale = Scale.Value;
    }


    // called every frame
    public virtual void Update()
        => UpdateEvent?.Invoke(this, EventArgs.Empty);


    // same as Update, but should be used for drawings
    public virtual void Draw(RenderTarget target)
        => DrawEvent?.Invoke(this, EventArgs.Empty);


    public Vec2f MapToAbsolute(Vec2f position)
        => position + Position;

    public Vec2f MapToRelative(Vec2f position)
        => position - Position;


    public virtual void Show() => Visible = true;
    public virtual void Hide() => Visible = false;


    public void Raise(uint amount = 1) => Priority += (int)amount;
    public void Lower(uint amount = 1) => Priority -= (int)amount;


    public void AddProperty(Property property) => _properties.Add(property);
    public bool RemoveProperty(Property property) => _properties.Remove(property);
    public bool HasProperty(Property property) => _properties.Contains(property);

    public Property GetProperty(string name)
        => _properties.Find(property => property.Name == name)
           ?? throw new ElementPropertyNotFoundException(name);

    public bool TryGetProperty(string name, [MaybeNullWhen(false)] out Property property)
    {
        try
        {
            property = GetProperty(name);
            return true;
        }
        catch
        {
            property = null;
            return false;
        }
    }


    public Property this[string name] => GetProperty(name);


    protected virtual void OnVisibilityChange()
        => VisibilityChangedEvent?.Invoke(this, EventArgs.Empty);


    protected virtual void OnPriorityChange()
        => PriorityChangedEvent?.Invoke(this, EventArgs.Empty);
}
