using SFML.Graphics;

using Latte.Core.Type;
using Latte.UI.Elements;
using Latte.Application;


namespace Latte.Test;




public sealed class MainSection : Section
{
    public MainSection()
    {
        var window = new WindowElement("Window", new Vec2f(), new Vec2f(600, 300))
        {
            Radius = 10f,

            BorderSize = 0f,
            BorderColor = Color.Magenta
        };


        var layout = new HorizontalLayoutElement(window, null)
        {
            Margin = 5f
        };


        var btn1 = new RadialButton(window, null, 10);
        var btn2 = new RadialButton(window, null, 10);
        var btn3 = new RadialButton(window, null, 10);

        btn1.Chain = [btn1, btn2, btn3];
        btn2.Chain = [btn1, btn2, btn3];
        btn3.Chain = [btn1, btn2, btn3];

        layout.Push(btn1);
        layout.Push(btn2);
        layout.Push(btn3);


        AddElements(window);
    }
}
