using System;
using System.Collections.Generic;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Elements.Primitives;


namespace Latte.Core.Application;


public static class App
{
    private static Window? _mainWindow;
    public static Window MainWindow
    {
        get => _mainWindow ?? throw AppNotInitializedException();    
        private set => _mainWindow = value;
    }

    private static View? _mainView;
    public static View MainView
    {
        get => _mainView ?? throw AppNotInitializedException();
        private set => _mainView = value;
    }
    
    private static View? _UIView;
    public static View UIView
    {
        get => _UIView ?? throw AppNotInitializedException();
        private set => _UIView = value;
    }
    
    private static bool _initialized;
    
    public static List<Element> Elements { get; set; } = [];


    public static void Init(VideoMode mode, string title, Font defaultFont, Styles styles = Styles.Default, ContextSettings settings = new())
    {
        if (_initialized)
            return;
        
        // workaround for enabling OpenTK (OpenGL Context) integration with SFML.
        // this should be ALWAYS initialized before MainWindow
        _ = new GameWindow(new(), new() { StartVisible = false });
        
        MainWindow = new(mode, title, styles, settings);
        MainWindow.Resized += (_, args) => OnWindowResized(new(args.Width, args.Height));

        MainView = new(MainWindow.GetView());
        UIView = new(MainView);
        
        TextElement.DefaultTextFont = defaultFont;
        
        _initialized = true;
    }
    

    public static void Update()
    {
        MainWindow.ProcessEvents();
        
        // use a separated view to draw UI elements
        MainWindow.SetView(UIView);
        
        foreach (Element element in Elements)
            element.Update();
        
        MainWindow.SetView(MainView);
    }


    public static void Draw()
    {
        MainWindow.SetView(UIView);
        
        foreach (Element element in Elements)
            element.Draw(MainWindow);
        
        MainWindow.SetView(MainView);
    }


    public static bool IsMouseOverAnyElementBound()
    {
        foreach (Element element in Elements)
            if (element.Visible && element.GetBounds().Contains(MainWindow.WorldMousePosition))
                return true;

        return false;
    }


    private static void OnWindowResized(Vector2u newSize)
    {
        MainView.Size = (Vector2f)newSize;
        
        UIView.Size = (Vector2f)newSize;
        UIView.Center = (Vector2f)newSize / 2f;
    }
    
    
    private static InvalidOperationException AppNotInitializedException()
        => new("App not initialized.");
}