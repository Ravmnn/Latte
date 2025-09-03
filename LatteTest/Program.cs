using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Type;
using Latte.Application;
using Latte.UI;
using Latte.UI.Elements;


namespace Latte.Test;


class Program
{
    private static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", EmbeddedResources.DefaultFont());
        App.Debugger.EnableKeyShortcuts = true;


        var window = new WindowElement("Window", new Vec2f(), new Vec2f(600, 300))
        {
            Radius = 10f,

            BorderSize = 0f,
            BorderColor = Color.Magenta
        };

        var rect = new RectangleElement(window, new Vec2f(), new Vec2f(200, 200))
        {
            Color = new ColorRGBA(230, 230, 230),
            BorderSize = 2f,
            BorderColor = Color.Red,

            Radius = 4f,

            Alignment = Alignment.Top | Alignment.Left,
            //AlignmentMargin = new Vec2f(30) }
        };

        new TextElement(rect, new Vec2f(), 30, "This is a text.")
        {
            Color = Color.Black,

            Alignment = Alignment.Center,

            Selection = { CanSelect = true }
        };

        new TextElement(window, new Vec2f(), 15, "This is a very, very long text.")
        {
            Color = Color.Black,

            Alignment = Alignment.HorizontalCenter | Alignment.Bottom,
            AlignmentMargin = new Vec2f(0, -10)
        };

        new TextInputElement(window, null, new Vec2f(150, 50))
        {
            BorderSize = 5f,

            Alignment = Alignment.VerticalCenter | Alignment.Right,
            AlignmentMargin = new Vec2f()
        };


        // var rect = new RectangleElement(null, new Vec2f(100, 200), new Vec2f(200, 200), 5f)
        // {
        //     Color = new ColorRGBA(255, 100, 100)
        // };
        //
        // Tween.Animate(rect.AbsolutePosition, new Vec2f(1500, 200), 6f, Easing.EaseInOutQuint)
        //     .ProgressEvent += (_, args) => rect.AbsolutePosition = args.Values.ToVec2f();
        //
        // Tween.Animate(rect.Color, new ColorRGBA(100, 255, 100), 6f, Easing.EaseInOutQuint)
        //     .ProgressEvent += (_, args) => rect.Color = args.Values.ToColor();


        App.AddElement(window);


        while (!App.ShouldQuit)
        {
            App.Update();
            App.Draw();
        }
    }
}
