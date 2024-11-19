using SFML.Window;
using SFML.Graphics;

using Latte.Sfml;
using Latte.Core;
using Latte.Core.Animation;
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
            AntialiasingLevel = 8
        });
        
        App.Elements.Add(new RectangleElement(null, new(), new(700, 700))
        {
            Color = { Value = Color.Green },
            Alignment = AlignmentType.Center,
            BorderSize = { Value = 4f },
            BorderColor = { Value = Color.White }
        });

        WindowElement window = new("Test Window", new(), new(400, 400));
        window.BorderSize.Value = 2f;
        window.BorderColor.Value = Color.White;
        window.Radius.Value = 5f;
        
        window.Position.Animate(new(1000, 200), 1f, EasingType.EaseOutQuint);
        
        App.Elements.Add(window);

        RectangleElement rect = new(App.Elements[0], new(), new(200, 200))
        {
            Alignment = AlignmentType.Center,
            
            Color = { Value = new(80, 80, 255) }
        };

        _ = new TextElement(rect, new(), 30, "this is a large text!!!")
        {
            Alignment = AlignmentType.Center
        };
        
        _ = new ButtonElement(App.Elements[0], new(), new(200, 90), "Press me!!")
        {
            Alignment = AlignmentType.BottomRight,
            AlignmentMargin = { Value = new(30, 40) },
            
            Radius = { Value = 3f }
        };
        
        _ = new ButtonElement(App.Elements[0], new(), new(300, 130), "Press me!!")
        {
            Alignment = AlignmentType.HorizontalCenter | AlignmentType.Top,
            AlignmentMargin = { Value = new(0, 10) },

            Radius = { Value = 10f }
        };

        RoundedRectangleShape rrect = new(new(300, 300), 10f, 16)
        {
            Position = new(200, 200),
            FillColor = new(255, 255, 100, 0),
            OutlineColor = new(0, 255, 50),
            OutlineThickness = 6f
        };
        
        while (App.MainWindow.IsOpen)
        {
            App.MainWindow.Clear();
            
            App.Update();
            App.Draw();
            
            App.MainWindow.Draw(rrect);
            
            App.MainWindow.Display();
        }
    }
}