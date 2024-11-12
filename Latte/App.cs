using System.Collections.Generic;

using Latte.Elements;


namespace Latte;


public static class App
{
    public static Window? MainWindow { get; set; }

    public static List<Element> Elements { get; set; } = [];
    

    public static void Update()
    {
        MainWindow?.ProcessEvents();
        
        foreach (Element element in Elements)
            element.Update();
    }


    public static void Draw()
    {
        if (MainWindow is null)
            return;
        
        MainWindow.Clear();
        
        foreach (Element element in Elements)
            element.Draw(MainWindow);
        
        MainWindow.Display();
    }
}