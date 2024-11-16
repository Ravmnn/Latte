using System;
using System.Collections.Generic;

using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Elements.Primitives;


namespace Latte.Application;


public static class App
{
    private static Window? _mainWindow;
    public static Window MainWindow
    {
        get => _mainWindow ?? throw new NullReferenceException("Main window not defined. Did you forget to call \"App.Init()\"?");    
        private set => _mainWindow = value;
    }
    
    private static bool _initialized;
    
    public static List<Element> Elements { get; set; } = [];


    public static void Init(Window mainWindow, Font defaultFont)
    {
        if (_initialized)
            return;
            
        _ = new GameWindow(new(), new() { StartVisible = false });
        
        MainWindow = mainWindow;
        TextElement.DefaultTextFont = defaultFont;
        
        _initialized = true;
    }
    

    public static void Update()
    {
        MainWindow.ProcessEvents();
        
        foreach (Element element in Elements)
            element.Update();
    }


    public static void Draw()
    {
        foreach (Element element in Elements)
            element.Draw(MainWindow);
    }
}