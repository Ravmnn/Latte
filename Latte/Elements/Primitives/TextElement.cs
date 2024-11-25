using System;

using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;


namespace Latte.Elements.Primitives;


public class TextElement : Element
{
    private static Font? s_defaultFont;
    
    
    public static Font DefaultTextFont
    {
        get => s_defaultFont ?? throw new InvalidOperationException("Default font is not defined.");
        set => s_defaultFont = value;
    }
    
    
    public override Transformable Transformable => SfmlText;
    
    public Text SfmlText { get; }
    
    public Property<string> Text { get; }
    public Property<Text.Styles> Style { get; }
    
    public Property<uint> Size { get; }
    public AnimatableProperty<Float> LetterSpacing { get; }
    public AnimatableProperty<Float> LineSpacing { get; }
    
    public AnimatableProperty<Float> BorderSize { get; }
    
    public AnimatableProperty<ColorRGBA> Color { get; }
    public AnimatableProperty<ColorRGBA> BorderColor { get; }
    

    public TextElement(Element? parent, Vec2f position, uint size, string text, Font? font = null) : base(parent)
    {
        BlocksMouseInput = false;
        
        SfmlText = new(text, font ?? DefaultTextFont);
        
        Text = new(this, nameof(Text), text);
        Style = new(this, nameof(Style), SFML.Graphics.Text.Styles.Regular);
        
        Size = new(this, nameof(Size), size);
        LetterSpacing = new(this, nameof(LetterSpacing), 1f);
        LineSpacing = new(this, nameof(LineSpacing), 1f);
        
        BorderSize = new(this, nameof(BorderSize), 0f);
        
        Color = new(this, nameof(Color), SFML.Graphics.Color.White);
        BorderColor = new(this, nameof(BorderColor), SFML.Graphics.Color.Black);
        
        RelativePosition.Set(position);
    }
    

    public override void Draw(RenderTarget target)
    {
        BeginDraw();
        target.Draw(SfmlText);
        EndDraw();
        
        base.Draw(target);
    }
    
    
    public override FloatRect GetBounds()
        => SfmlText.GetGlobalBounds();


    public override Vec2f GetAlignmentPosition(Alignments alignment)
    {
        // text local bounds work quite different
        // https://learnsfml.com/basics/graphics/how-to-center-text/#set-a-string
        
        FloatRect localBounds = SfmlText.GetLocalBounds();
        
        Vec2f position = base.GetAlignmentPosition(alignment);
        position -= localBounds.Position;
        
        return position;
    }


    protected override void UpdateSfmlProperties()
    {
        base.UpdateSfmlProperties();

        // round to avoid blurry text
        Transformable.Position = new(MathF.Round(AbsolutePosition.X), MathF.Round(AbsolutePosition.Y));
        Transformable.Origin = new(MathF.Round(Origin.Value.X), MathF.Round(Origin.Value.Y));

        SfmlText.DisplayedString = Text;
        SfmlText.Style = Style.Value;
        
        SfmlText.CharacterSize = Size.Value;
        SfmlText.LetterSpacing = LetterSpacing.Value;
        SfmlText.LineSpacing = LineSpacing.Value;
        
        SfmlText.FillColor = Color.Value;
        SfmlText.OutlineColor = BorderColor.Value;
    }
}