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
        App.Debugger!.EnableKeyShortcuts = true;


        var window = new WindowElement("Window", new Vec2f(), new Vec2f(600, 300))
        {
            Radius = 10f,

            BorderSize = 0f,
            BorderColor = Color.Magenta
        };


        var slider = new SliderElement(window, null, 0, 300, Orientation.Vertical);


        App.AddElement(window);


        while (!App.ShouldQuit)
        {
            Console.WriteLine(slider.Value);

            App.Update();
            App.Draw();
        }


        App.Deinit();
    }
}
