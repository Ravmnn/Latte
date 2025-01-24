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

        ButtonElement addButton = new(rect, new(), new(100, 20), "Add")
        {
            Alignment = { Value = Alignment.HorizontalCenter | Alignment.Bottom },
            AlignmentMargin = { Value = new(0, -10f) },

            // TODO: globally setting the origin to the center of the element may improve animations that change size and scale

            Color = { Value = new(150, 150, 255) }
        };

        addButton.Color.Set(new(255, 150, 150));

        ScrollAreaElement scrollAreaElement = new(rect, new(), new(200, 200), true, true)
        {
            Color = { Value = new(200, 100, 100, 40) },

            Alignment = { Value = Alignment.Center }
        };

        GridLayoutElement grid = new(scrollAreaElement, new(), 5, 10, 35, 35)
        {
            GrowDirection = GridLayoutGrowDirection.Vertical
        };


        for (int i = 0; i < 100; i++)
            AddButtonToLayout(grid);

        addButton.MouseClickEvent += (_, _) => AddButtonToLayout(grid);

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
