using SFML.Graphics;

using Latte.Core.Type;
using Latte.UI;
using Latte.UI.Elements;
using Latte.Application;


namespace Latte.Test;




public sealed class MainSection : Section
{
    public MainSection()
    {
        var window = new WindowElement("Window", new Vec2f(), new Vec2f(600, 300))
        {
            Radius = 50f,

            BorderSize = 0f,
            BorderColor = Color.Magenta
        };

        _ = new SliderElement(window, null, 0, 300, Orientation.Vertical);


        AddElement(window);
    }
}
