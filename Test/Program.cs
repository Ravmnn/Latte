using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Test;


class Program
{
    static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", new("../../../resources/Itim-Regular.ttf"), settings: new()
        {
            AntialiasingLevel = 16
        });

        WindowElement window = new("Test Window", new(), new(600, 600))
        {
            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.White },
            Radius = { Value = 5f }
        };

        GridLayout layout = new(window, new(), 8, 5, 50f, 50f)
        {
            Alignment = { Value = Alignments.Center }
        };

        for (int i = 0; i < 40; i++)
            layout.AddElement(new ButtonElement(null, new(), new(30, 30), "Btn")
            {
                Alignment = { Value = Alignments.Center }
            });

        App.AddElement(window);
        
        while (App.Window.IsOpen)
        {
            App.Window.Clear();
            
            App.Update();
            App.Draw();
            
            App.Window.Display();
        }
    }
}