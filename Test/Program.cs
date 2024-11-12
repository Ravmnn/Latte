using SFML.Window;

using Latte.Elements;


namespace Latte.Test;


class Program
{
    static void Main()
    {
        App.MainWindow = new(VideoMode.DesktopMode, "Latte Test");
        TextElement.DefaultTextFont = new("../../../resources/Itim-Regular.ttf");
        
        App.Elements.Add(new ButtonElement(null, new(), new(200, 70), "Press me!")
        {
            Alignment = AlignmentType.Center
        });

        while (App.MainWindow.IsOpen)
        {
            App.Update();
            App.Draw();
        }
    }
}