using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


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
        
        // WindowElement window = new("Test Window", new(), new(600, 600))
        // {
        //     BorderSize = { Value = 2f },
        //     BorderColor = { Value = Color.White },
        //     Radius = { Value = 5f }
        // };
        //
        // var addButton = new ButtonElement(window, new(), new(120, 80), "Add a new button")
        // {
        //     Alignment = { Value = Alignments.BottomRight },
        //     AlignmentMargin = { Value = new(-10, -10) }
        // };
        //
        // GridLayoutElement layoutElement = new(window, new(), 5, 5, 50f, 50f)
        // {
        //     Alignment = { Value = Alignments.Center },
        //     GrowDirection = GridLayoutGrowDirection.Vertically
        // };
        //
        // for (int i = 0; i < 25; i++)
        //     AddButtonToLayout(layoutElement);

        // addButton.MouseUpEvent += (_, _) => AddButtonToLayout(layoutElement);

        WindowElement rect = new("this is a text", new(), new(600, 400));


        _ = new ButtonElement(rect, new(), new(200, 50), "long text yay")
        {
            Alignment = { Value = Alignments.Center }
        };
        
        
        while (App.Window.IsOpen)
        {
            App.Window.Clear();
            
            App.Update();
            App.Draw();
            
            App.Window.Display();
        }
    }
}