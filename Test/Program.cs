using SFML.Window;
using SFML.Graphics;

using Latte.Core;
using Latte.Core.Application;
using Latte.Elements;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Test;


class Program
{
    static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test", new("../../../resources/Itim-Regular.ttf"), settings: new()
        {
            AntialiasingLevel = 16
        });
        
        RectangleElement rectangle = new(null, new(), new(700, 700))
        {
            Color = { Value = Color.Green },
            Alignment = { Value = Alignments.Center },
            BorderSize = { Value = 4f },
            BorderColor = { Value = Color.White }
        };

        WindowElement window = new("Test Window", new(), new(400, 400))
        {
            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.White },
            Radius = { Value = 5f }
        };

        _ = new ButtonElement(window, new(), new(130, 70), "Button")
        {
            Alignment = { Value = Alignments.Center }
        };
        
        RectangleElement rect = new(rectangle, new(), new(200, 200))
        {
            Alignment = { Value = Alignments.Center},
            
            Color = { Value = new(80, 80, 255) }
        };

        _ = new TextElement(rect, new(), 30, "this is a large text!!!")
        {
            Alignment = { Value = Alignments.Center}
        };
        
        _ = new ButtonElement(rectangle, new(), new(200, 90), "Press me!!")
        {
            Alignment = { Value = Alignments.BottomRight},
            AlignmentMargin = { Value = new(30, 40) },
            
            Radius = { Value = 3f }
        };
        
        _ = new ButtonElement(rectangle, new(), new(300, 130), "Press me!!")
        {
            Alignment = { Value = Alignments.HorizontalCenter | Alignments.Top },
            AlignmentMargin = { Value = new(0, 10) },

            Radius = { Value = 5f }
        };
        
        App.AddElement(rectangle);
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