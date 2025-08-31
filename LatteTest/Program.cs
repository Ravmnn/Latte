using SFML.Window;
using SFML.Graphics;

using Latte.Core.Type;
using Latte.Application;
using Latte.Application.Elements;
using Latte.Application.Elements.Behavior;
using Latte.Application.Elements.Primitives;
using Latte.Application.Elements.Primitives.Shapes;


namespace Latte.Test;


class Program
{
    private static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", EmbeddedResources.DefaultFont());
        App.Debugger.EnableKeyShortcuts = true;


        var window = new WindowElement("Window", new Vec2f(), new Vec2f(600, 300))
        {
            Radius = { Value = 10f },

            BorderSize = { Value = 0f },
            BorderColor = { Value = Color.Magenta }
        };

        var rect = new RectangleElement(window, new Vec2f(), new Vec2f(200, 200))
        {
            Color = { Value = new ColorRGBA(230, 230, 230) },
            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.Red },

            Radius = { Value = 4f },

            Alignment = { Value = Alignment.Top | Alignment.Left },
            //AlignmentMargin = { Value = new Vec2f(30) }
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

        new TextInputElement(window, null, new Vec2f(150, 50))
        {
            BorderSize = { Value = 5f },

            Alignment = { Value = Alignment.VerticalCenter | Alignment.Right },
            AlignmentMargin = { Value = new Vec2f() }
        };


        App.AddElement(window);


        while (!App.ShouldQuit)
        {
            App.Update();
            App.Draw();
        }
    }
}
