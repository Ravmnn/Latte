using Latte.Core.Type;


namespace Latte.UI.Elements;




public class VerticalLayoutElement(Element? parent, Vec2f? position) : LayoutElement(parent, position)
{
    public float Margin { get; set; }




    protected override void UpdateElements()
    {
        var currentPosition = new Vec2f();
        var highestWidth = 0f;

        foreach (var element in Elements)
        {
            UpdateElement(element, currentPosition);

            var bounds = element.GetBounds();
            currentPosition.Y += bounds.Height + Margin;

            if (bounds.Width > highestWidth)
                highestWidth = bounds.Width;
        }

        Size = new Vec2f(highestWidth, currentPosition.Y);
    }
}
