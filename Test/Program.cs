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
        
        App.Elements.Add(new RectangleElement(null, new(), new(700, 700))
        {
            Color = { Value = Color.Green },
            Alignment = { Value = Alignments.Center },
            BorderSize = { Value = 4f },
            BorderColor = { Value = Color.White }
        });

        WindowElement window = new("Test Window", new(), new(400, 400))
        {
            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.White },
            Radius = { Value = 5f }
        };
        
        App.Elements.Add(window);
        
        RectangleElement rect = new(App.Elements[0], new(), new(200, 200))
        {
            Alignment = { Value = Alignments.Center},
            
            Color = { Value = new(80, 80, 255) }
        };

        _ = new TextElement(rect, new(), 30, "this is a large text!!!")
        {
            Alignment = { Value = Alignments.Center}
        };
        
        _ = new ButtonElement(App.Elements[0], new(), new(200, 90), "Press me!!")
        {
            Alignment = { Value = Alignments.BottomRight},
            AlignmentMargin = { Value = new(30, 40) },
            
            Radius = { Value = 3f }
        };
        
        _ = new ButtonElement(App.Elements[0], new(), new(300, 130), "Press me!!")
        {
            Alignment = { Value = Alignments.HorizontalCenter | Alignments.Top },
            AlignmentMargin = { Value = new(0, 10) },

            Radius = { Value = 5f }
        };
        
        window.Lower();
        
        while (App.Window.IsOpen)
        {
            App.Window.Clear();
            
            App.Update();
            App.Draw();
            
            App.Window.Display();
        }
    }
}