using System;

using SFML.System;
using SFML.Graphics;


namespace Latte.Elements.Primitives;


public class TextElement : Element
{
    private static Font? _defaultFont;
    public static Font DefaultTextFont
    {
        get => _defaultFont ?? throw new InvalidOperationException("Default font is not defined.");
        set => _defaultFont = value;
    }
    
    public override Transformable Transformable => Text;
    
    public Text Text { get; protected set; } 
    

    public TextElement(Element? parent, Vector2f position, uint size, string text, Font? font = null) : base(parent)
    {
        Text = new(text, font ?? DefaultTextFont)
        {
            FillColor = Color.White,
            CharacterSize = size
        };
        
        Position = position;
    }
    

    public override void Draw(RenderTarget target)
    {
        if (!Visible)
            return;
        
        BeginDraw();
        target.Draw(Text);
        EndDraw();
        
        base.Draw(target);
    }
    
    
    public override FloatRect GetBounds()
        => Text.GetGlobalBounds();


    public override Vector2f GetAlignmentPosition(AlignmentType alignment)
    {
        // text local bounds work quite different
        // https://learnsfml.com/basics/graphics/how-to-center-text/#set-a-string
        
        FloatRect localBounds = Text.GetLocalBounds();
        
        Vector2f position = base.GetAlignmentPosition(alignment);
        position -= localBounds.Position;
        
        return position;
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        // round to avoid blurry text
        Transformable.Position = new(MathF.Round(AbsolutePosition.X), MathF.Round(AbsolutePosition.Y));
        Transformable.Origin = new(MathF.Round(Origin.X), MathF.Round(Origin.Y));
    }
}