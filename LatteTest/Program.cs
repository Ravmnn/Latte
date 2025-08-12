using SFML.Window;
using SFML.Graphics;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Behavior;
using Latte.Elements.Primitives;


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
        var font = new Font("resources/Roboto-Regular.ttf");
        font.SetSmooth(false);

        App.Init(VideoMode.DesktopMode, "Latte Test", font, settings: new ContextSettings
        {
            AntialiasingLevel = 8,
            DepthBits = 24,
            StencilBits = 8
        });

        //App.DebugOptions = DebugOption.RenderBounds | DebugOption.RenderBoundsDimensions | DebugOption.OnlyTrueHoveredElement;
        App.Debugger.EnableKeyShortcuts = true;


        while (!App.ShouldQuit)
        {
            App.Update();
            App.Draw();
        }
    }
}
