using SFML.Window;
using SFML.Graphics;

using Latte.Application;
using Latte.Elements;
using Latte.Elements.Shapes;


namespace Latte.Test;


class Program
{
    static void Main()
    {
        App.MainWindow = new(VideoMode.DesktopMode, "Latte Test");
        TextElement.DefaultTextFont = new("../../../resources/Itim-Regular.ttf");
        
        App.Elements.Add(new RectangleElement(null, new(), new(600, 600))
        {
            Alignment = AlignmentType.Center
        });
        
        App.Elements.Add(new RectangleElement(App.Elements[0], new(), new(300, 300))
        {
            Color = Color.Blue,
            
            Alignment = AlignmentType.BottomRight,
            AlignmentMargin = new(200, 200),
            ShouldDrawElementBoundaries = true
        });
        
        while (App.MainWindow.IsOpen)
        {
            App.Update();
            App.Draw();
        }
    }
}