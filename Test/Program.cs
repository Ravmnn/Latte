using SFML.Window;
using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Application;
using Latte.Elements;
using Latte.Elements.Shapes;


namespace Latte.Test;


class Program
{
    static void Main()
    {
        _ = new GameWindow(new(), new() { StartVisible = false });
        
        App.MainWindow = new(VideoMode.DesktopMode, "Latte Test");
        TextElement.DefaultTextFont = new("../../../resources/Itim-Regular.ttf");
        
        App.Elements.Add(new RectangleElement(null, new(), new(600, 600))
        {
            Color = Color.Green,
            
            Alignment = AlignmentType.Center,
            ShouldDrawElementBoundaries = false,
            BorderSize = 10f,
            BorderColor = Color.White,
        });
        
        _ = new ButtonElement(App.Elements[0], new(), new(200, 90), "Press me!!")
        {
            Alignment = AlignmentType.BottomRight,
            AlignmentMargin = new(30, 20),
            ShouldDrawElementBoundaries = false,
        };

        App.Elements.Add(new TextElement(null, new(), 50, "this is a large text.")
        {
            Alignment = AlignmentType.TopLeft,
            AlignmentMargin = new(40, 40),
            ShouldDrawElementBoundaries = false
        });
        
        while (App.MainWindow.IsOpen)
        {
            App.Update();
            App.Draw();
        }
    }
}