using System;
using System.Collections.Generic;
using System.Diagnostics;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Core.Type;
using Latte.Elements.Primitives;


namespace Latte.Core.Application;


public static class App
{
    private static Window? s_window;
    private static View? s_mainView;
    private static View? s_elementView;
    
    private static bool s_initialized;
    
    private static Vec2i s_lastMousePosition;
    private static Vec2f s_lastWorldMousePosition;
    
    private static readonly Stopwatch s_deltaTimeStopwatch;
    
    
    public static Window Window
    {
        get => s_window ?? throw AppNotInitializedException();    
        private set => s_window = value;
    }

    public static View MainView
    {
        get => s_mainView ?? throw AppNotInitializedException();
        private set => s_mainView = value;
    }
    
    public static View ElementView
    {
        get => s_elementView ?? throw AppNotInitializedException();
        private set => s_elementView = value;
    }
    
    public static Vec2i MousePosition { get; private set; }
    public static Vec2i MousePositionDelta => MousePosition - s_lastMousePosition;
    public static Vec2f WorldMousePosition { get; private set; }
    public static Vec2f WorldMousePositionDelta => WorldMousePosition - s_lastWorldMousePosition;
    
    public static TimeSpan DeltaTime { get; private set; }
    public static double DeltaTimeInSeconds => DeltaTime.TotalSeconds;
    public static int DeltaTimeInMilliseconds => DeltaTime.Milliseconds;
    
    public static List<Element> Elements { get; }
    

    static App()
    {
        s_lastMousePosition = new();
        s_lastWorldMousePosition = new();
        
        MousePosition = new();
        WorldMousePosition = new();
        
        s_deltaTimeStopwatch = new();
        
        DeltaTime = TimeSpan.Zero;
        
        Elements = [];
    }
    

    public static void Init(VideoMode mode, string title, Font defaultFont, Styles styles = Styles.Default, ContextSettings settings = new())
    {
        if (s_initialized)
        {
            Console.WriteLine("App has already been initialized.");
            return;
        }
        
        // workaround for enabling OpenTK (OpenGL Context) integration with SFML.
        // this should be ALWAYS initialized before MainWindow
        _ = new GameWindow(new(), new() { StartVisible = false });
        
        Window = new(mode, title, styles, settings);
        Window.Resized += (_, args) => OnWindowResized(new(args.Width, args.Height));

        MainView = new(Window.GetView());
        ElementView = new(MainView); // TODO: enum RenderMode: Pinned or Unpinned
        
        TextElement.DefaultTextFont = defaultFont;
        
        s_deltaTimeStopwatch.Start();

        s_initialized = true;
    }
    

    public static void Update()
    {
        UpdateDeltaTime();
        UpdateMousePositionProperties();
        
        Window.ProcessEvents();
        Window.SetView(ElementView);

        foreach (Element element in Elements)
            element.Update();
        
        Window.SetView(MainView);
    }

    private static void UpdateMousePositionProperties()
    {
        s_lastMousePosition = MousePosition;
        s_lastWorldMousePosition = WorldMousePosition;

        MousePosition = Window.MousePosition;
        WorldMousePosition = Window.WorldMousePosition;
    }

    private static void UpdateDeltaTime()
    {
        DeltaTime = s_deltaTimeStopwatch.Elapsed;
        s_deltaTimeStopwatch.Restart();
    }


    public static void Draw()
    {
        // use a separated view to draw UI elements
        Window.SetView(ElementView);
        
        foreach (Element element in Elements)
            element.Draw(Window);
        
        Window.SetView(MainView);
    }


    public static bool IsMouseOverAnyElementBound()
    {
        foreach (Element element in Elements)
            if (element.Visible && element.GetBounds().Contains((Vector2f)Window.WorldMousePosition))
                return true;

        return false;
    }


    private static void OnWindowResized(Vec2u newSize)
    {
        MainView.Size = newSize;
        
        ElementView.Size = newSize;
        ElementView.Center = (Vector2f)newSize / 2f;
    }
    
    
    private static InvalidOperationException AppNotInitializedException()
        => new("App not initialized.");
}