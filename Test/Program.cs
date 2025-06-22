using SFML.Window;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Primitives;
using SFML.Graphics;


namespace Latte.Test;


class Program
{
    private static int s_counter;


    private static void AddButtonToLayout(GridLayoutElement layoutElement)
        => layoutElement.AddElementAtEnd(new ButtonElement(null, new Vec2f(), new Vec2f(30, 30), $"{s_counter++}")
        {
            Alignment = { Value = Alignment.Center }
        });


    private static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", new Font("../../../resources/Itim-Regular.ttf"), settings: new ContextSettings
        {
            AntialiasingLevel = 8,
            DepthBits = 24,
            StencilBits = 8
        });

        //App.DebugOptions = DebugOption.RenderBounds | DebugOption.RenderBoundsDimensions | DebugOption.OnlyTrueHoveredElement;
        App.Debugger.EnableKeyShortcuts = true;

        var rect = new WindowElement("this is a text", new Vec2f(), new Vec2f(600, 400))
        {
            Title =
            {
                Color = { Value = new ColorRGBA(0, 0, 0)}
            },

            Color = { Value = new ColorRGBA(255, 255, 255) }
        };

        var button = new ButtonElement(rect, new Vec2f(), new Vec2f(200, 80), "Button")
        {
            Alignment = { Value = Alignment.Center },

            Color = { Value = new ColorRGBA(150, 150, 255) },
            Radius = { Value = 8f },
        };

        button.Text!.SizePolicy.Value = SizePolicyType.FitParent;

        App.AddElement(rect);

        while (!App.ShouldQuit)
        {
            App.Update();

            App.Window.Clear();
            App.Draw();
            App.Window.Display();
        }
    }
}
