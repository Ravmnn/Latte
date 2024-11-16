using SFML.Window;
using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Application;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;


namespace Latte.Test;


class Program
{
    static void Main()
    {
        _ = new GameWindow(new(), new() { StartVisible = false });
        
        App.MainWindow = new(VideoMode.DesktopMode, "Latte Test");
        TextElement.DefaultTextFont = new("../../../resources/Itim-Regular.ttf");
        
        App.Elements.Add(new RectangleElement(null, new(), new(700, 700))
        {
            Color = Color.Green,
            Alignment = AlignmentType.Center,
            BorderSize = 4f,
            BorderColor = Color.White,
            ShouldDrawElementBoundaries = false,
            ShouldDrawClipArea = false
        });

        RectangleElement rect = new(App.Elements[0], new(), new(200, 200))
        {
            Alignment = AlignmentType.Left | AlignmentType.VerticalCenter,
            AlignmentMargin = new(-40, 0),
            
            Color = new(80, 80, 255)
        };

        _ = new TextElement(rect, new(), 30, "this is a large text!!!")
        {
            Alignment = AlignmentType.Center
        };
        
        _ = new ButtonElement(App.Elements[0], new(), new(200, 90), "Press me!!")
        {
            Alignment = AlignmentType.BottomRight,
            AlignmentMargin = new(30, 40),
            ShouldDrawElementBoundaries = false,
            ShouldDrawClipArea = false
        };
        
        while (App.MainWindow.IsOpen)
        {
            App.Update();
            App.Draw();
        }
    }
}