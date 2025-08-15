using SFML.Window;
using SFML.Graphics;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Behavior;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Test;


class Program
{
    private static void Main()
    {

        // TODO: embed into Latte
        var font = new Font("resources/Roboto-Regular.ttf");
        font.SetSmooth(false);

        App.Init(VideoMode.DesktopMode, "Latte Test", font);
        App.Debugger.EnableKeyShortcuts = true;


        var window = new WindowElement("Window", new Vec2f(), new Vec2f(300, 300))
        {
            Radius = { Value = 10f },

            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.Magenta }
        };

        var rect = new RectangleElement(window, new Vec2f(), new Vec2f(200, 200))
        {
            Color = { Value = Color.Green },
            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.Red },

            Radius = { Value = 4f },

            Alignment = { Value = Alignment.VerticalCenter | Alignment.Left },
            AlignmentMargin = { Value = new Vec2f(-50) }
        };

        var button = new ButtonElement(rect, new Vec2f(), new Vec2f(80, 80), "Button")
        {
            Alignment = { Value = Alignment.TopLeft },
            AlignmentMargin = { Value = new Vec2f(-15, -15) }
        };


        App.AddElement(window);


        while (!App.ShouldQuit)
        {
            App.Update();
            App.Draw();
        }
    }
}
