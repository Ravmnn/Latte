using Latte.Application;
using Latte.Application.Elements;
using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives;
using Latte.Application.Elements.Primitives.Shapes;
using SFML.Window;
using SFML.Graphics;
using Latte.Core.Type;


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
            Color = { Value = new ColorRGBA(230, 230, 230) },
            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.Red },

            Radius = { Value = 4f },

            Alignment = { Value = Alignment.Center }
        };

        new TextElement(rect, new Vec2f(), 30, "This is a text.")
        {
            Color = { Value = Color.Black },

            Alignment = { Value = Alignment.Center },

            Selection = { CanSelect = true }
        };

        new TextElement(window, new Vec2f(), 15, "This is a very, very long text.")
        {
            Color = { Value = Color.Black },

            Alignment = { Value = Alignment.HorizontalCenter | Alignment.Bottom },
            AlignmentMargin = { Value = new Vec2f(0, -10) }
        };


        App.AddElement(window);


        while (!App.ShouldQuit)
        {
            App.Update();
            App.Draw();
        }
    }
}
