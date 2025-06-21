using SFML.Window;

using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Test;


class Program
{
    private static int s_counter;


    private static void AddButtonToLayout(GridLayoutElement layoutElement)
        => layoutElement.AddElementAtEnd(new ButtonElement(null, new(), new(30, 30), $"{s_counter++}")
        {
            Alignment = { Value = Alignment.Center }
        });


    private static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", new("../../../resources/Itim-Regular.ttf"), settings: new()
        {
            AntialiasingLevel = 8,
            DepthBits = 24,
            StencilBits = 8
        });

        //App.DebugOptions = DebugOption.RenderBounds | DebugOption.RenderBoundsDimensions | DebugOption.OnlyTrueHoveredElement;
        App.Debugger.EnableKeyShortcuts = true;

        WindowElement rect = new("this is a text", new(), new(600, 400))
        {
            Title =
            {
                Color = { Value = new(0, 0, 0)}
            },

            Color = { Value = new(255, 255, 255) }
        };

        ButtonElement button = new(rect, new(), new(200, 80), "Button")
        {
            Alignment = { Value = Alignment.Center },

            Color = { Value = new(150, 150, 255) },
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
