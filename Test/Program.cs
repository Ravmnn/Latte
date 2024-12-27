using System.Drawing;
using SFML.Window;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;
using SFML.Graphics;


namespace Latte.Test;


class Program
{
    private static void AddButtonToLayout(GridLayoutElement layoutElement)
        => layoutElement.AddElement(new ButtonElement(null, new(), new(30, 30), "Btn")
        {
            Alignment = { Value = Alignments.Center }
        });


    static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", new("../../../resources/Itim-Regular.ttf"), settings: new()
        {
            AntialiasingLevel = 16
        });

        WindowElement rect = new("this is a text", new(), new(600, 400))
        {
            Title =
            {
                Color = { Value = new(0, 0, 0)}
            },

            Color = { Value = new(255, 255, 255) }
        };

        ScrollAreaElement scrollAreaElement = new(rect, new(), new(200, 200))
        {
            Color = { Value = new(200, 100, 100, 40) },

            Alignment = { Value = Alignments.Center}
        };

        GridLayoutElement grid = new(scrollAreaElement, new(), 5, 5, 35, 35)
        {
            GrowDirection = GridLayoutGrowDirection.Vertically
        };

        for (int i = 0; i < 100; i++)
            AddButtonToLayout(grid);

        App.AddElement(rect);


        while (App.Window.IsOpen)
        {
            App.Window.Clear();

            App.Update();
            App.Draw();

            // Debug.DrawLineRect(App.Window, scrollAreaElement.Viewport, SFML.Graphics.Color.Blue);

            App.Window.Display();
        }
    }
}
