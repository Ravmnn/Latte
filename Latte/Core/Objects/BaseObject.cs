using System;

using SFML.Graphics;

using Latte.Core.Type;
using Latte.Rendering;


namespace Latte.Core.Objects;




public class BaseObjectEventArgs(BaseObject? @object) : EventArgs
{
    public BaseObject? @Object { get; } = @object;
}




public abstract class BaseObject : IUpdateable, IDrawable, ISfmlObject
{


    public event EventHandler? UpdateEvent;




    public event EventHandler? DrawEvent;




    public abstract Transformable SfmlTransformable { get; }
    public abstract Drawable SfmlDrawable { get; }


    public Vec2f Position { get; set; }
    public Vec2f Origin { get; set; }
    public float Rotation { get; set; }
    public Vec2f Scale { get; set; }




    public Effect? Effect { get; set; }

    public BaseObjectAttributeManager Attributes { get; }




    public bool Initialized { get; private set; }




    private bool _visible;
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

    public event EventHandler? VisibilityChangedEvent;




    private int _priority;
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

    public event EventHandler? PriorityChangedEvent;




    public event EventHandler? SetupEvent;
    public event EventHandler? ConstantUpdateEvent;




    public virtual bool CanUpdate => Visible;
    public virtual bool CanDraw => Initialized && Visible;



    public BaseObject()
    {
        Attributes = new BaseObjectAttributeManager(this);

        Visible = true;

        Position = new Vec2f();
        Origin = new Vec2f();
        Scale = new Vec2f(1f, 1f);
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




    // called every frame
    public virtual void Update()
        => UpdateEvent?.Invoke(this, EventArgs.Empty);




    // same as Update, but should be used for drawings
    public virtual void Draw(IRenderer renderer)
    {
        renderer.Render(SfmlDrawable, Effect);

        DrawEvent?.Invoke(this, EventArgs.Empty);
    }




    public virtual void UpdateSfmlProperties()
    {
        SfmlTransformable.Position = Position;
        SfmlTransformable.Origin = Origin;
        SfmlTransformable.Rotation = Rotation;
        SfmlTransformable.Scale = Scale;
    }




    public Vec2f MapToAbsolute(Vec2f position)
        => position + Position;

    public Vec2f MapToRelative(Vec2f position)
        => position - Position;




    public abstract FloatRect GetBounds();




    public virtual void Show() => Visible = true;
    public virtual void Hide() => Visible = false;




    public void Raise(uint amount = 1) => Priority += (int)amount;
    public void Lower(uint amount = 1) => Priority -= (int)amount;




    protected virtual void OnVisibilityChange()
        => VisibilityChangedEvent?.Invoke(this, EventArgs.Empty);


    protected virtual void OnPriorityChange()
        => PriorityChangedEvent?.Invoke(this, EventArgs.Empty);
}
