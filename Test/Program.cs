using SFML.Window;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Test;


class Program
{
    private static void AddButtonToLayout(GridLayoutElement layoutElement)
        => layoutElement.AddElementAtEnd(new ButtonElement(null, new(), new(30, 30), "Btn")
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

        ButtonElement removeButton = new(rect, new(), new(100, 40), "Add")
        {
            Alignment = { Value = Alignments.BottomRight },
            AlignmentMargin = { Value = new(-10, -5) }
        };


        ScrollAreaElement scrollAreaElement = new(rect, new(), new(200, 200))
        {
            Color = { Value = new(200, 100, 100, 40) },

            Alignment = { Value = Alignments.Center }
        };

        GridLayoutElement grid = new(scrollAreaElement, new(), 5, 5, 35, 35)
        {
            GrowDirection = GridLayoutGrowDirection.Vertically
        };

        for (int i = 0; i < 10; i++)
            AddButtonToLayout(grid);

        removeButton.MouseUpEvent += (_, _) =>
        {
            grid.DeleteLastElement();
        };

        // scrollAreaElement.ScrollOffset.Y = scrollAreaElement.GetChildrenBounds().Size.Y / 2f;

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
