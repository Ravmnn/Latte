using Latte.Core;
using SFML.Window;

using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Test;


class Program
{
    static int counter;


    private static void AddButtonToLayout(GridLayoutElement layoutElement)
        => layoutElement.AddElementAtEnd(new ButtonElement(null, new(), new(30, 30), $"{counter++}")
        {
            Alignment = { Value = Alignment.Center }
        });


    static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", new("../../../resources/Itim-Regular.ttf"), settings: new()
        {
            AntialiasingLevel = 8,
            DepthBits = 24,
            StencilBits = 8
        });

        //App.DebugOptions = DebugOption.RenderBounds | DebugOption.RenderBoundsDimensions | DebugOption.OnlyTrueHoveredElement;
        App.EnableDebugShortcuts = true;

        WindowElement rect = new("this is a text", new(), new(600, 400))
        {
            Title =
            {
                Color = { Value = new(0, 0, 0)}
            },

            Color = { Value = new(255, 255, 255) }
        };

        ButtonElement addButton = new(rect, new(), new(100, 40), "Add")
        {
            Alignment = { Value = Alignment.BottomRight },
            AlignmentMargin = { Value = new(-10, -5) }
        };


        ScrollAreaElement scrollAreaElement = new(rect, new(), new(200, 200), true, true)
        {
            Color = { Value = new(200, 100, 100, 40) },

            Alignment = { Value = Alignment.Center }
        };

        GridLayoutElement grid = new(scrollAreaElement, new(), 5, 10, 35, 35)
        {
            GrowDirection = GridLayoutGrowDirection.Vertically
        };

        for (int i = 0; i < 100; i++)
            AddButtonToLayout(grid);

        addButton.MouseUpEvent += (_, _) =>
        {
            AddButtonToLayout(grid);
            App.AddElement(grid);
        };

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
