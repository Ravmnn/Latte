using Latte.Core.Type;


namespace Latte.UI.Elements;




public class HorizontalLayoutElement(Element? parent, Vec2f? position)
    : LayoutElement(parent, position)
{
    public float Margin { get; set; }




    protected override void UpdateElements()
    {
        var currentPosition = new Vec2f();
        var highestHeight = 0f;

        foreach (var element in Elements)
        {
            UpdateElement(element, currentPosition);

            var bounds = element.GetBounds();
            currentPosition.X += bounds.Width + Margin;

            if (bounds.Height > highestHeight)
                highestHeight = bounds.Height;
        }

        Size = new Vec2f(currentPosition.X, highestHeight);
    }
}
