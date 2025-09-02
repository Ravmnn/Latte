using Latte.Core.Type;
using Latte.Application.Elements.Primitives;
using Latte.Debugging.Inspection;


namespace Latte.Debugging.Elements;


public class InspectionFrameElement : ScrollAreaElement
{
    private InspectionData _data;


    public TextElement NameText { get; }
    public TextElement DataText { get; }

    public InspectionData Data
    {
        get => _data;
        set
        {
            _data = value;

            NameText.Text = _data.Name;
            DataText.Text = _data.Data;
        }
    }


    public InspectionFrameElement(InspectionData data)
        : base(null, new Vec2f(), new Vec2f(350, 350), true, true)
    {
        _data = data;

        NameText = new TextElement(this, new Vec2f(), 10, data.Name)
        {
            Alignment = Application.Elements.Behavior.Alignment.HorizontalCenter | Application.Elements.Behavior.Alignment.Top,
            AlignmentMargin = new Vec2f(0, 10)
        };

        DataText = new TextElement(this, new Vec2f(5, 20), 13, data.Data);

        Alignment = Application.Elements.Behavior.Alignment.Center;

        Color = new ColorRGBA(100, 100, 100, 150);
        Radius = 5f;
    }
}
