using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Primitives;


namespace Latte.Core.Application;


public enum RenderMode
{
    Pinned,
    Unpinned
}


public static class App
{
    private static Window? s_window;
    private static View? s_mainView;
    private static View? s_elementView;
    
    private static bool s_initialized;
    
    private static Vec2i s_lastMousePosition;
    private static Vec2f s_lastElementViewMousePosition;
    private static Vec2f s_lastMainViewMousePosition;
    
    private static readonly Stopwatch s_deltaTimeStopwatch;

    private static bool _mouseInputWasCaught;
    
    
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
        get
        {
            if (RenderMode == RenderMode.Unpinned)
                return MainView;
            
            return s_elementView ?? throw AppNotInitializedException();
        }
        
        private set => s_elementView = value;
    }

    public static RenderMode RenderMode { get; set; }
    
    public static Vec2i MousePosition { get; private set; }
    public static Vec2i MousePositionDelta => MousePosition - s_lastMousePosition;
    public static Vec2f ElementViewMousePosition { get; private set; }
    public static Vec2f MainViewMousePosition { get; private set; }
    public static Vec2f ElementViewMousePositionDelta => ElementViewMousePosition - s_lastElementViewMousePosition;
    public static Vec2f MainViewMousePositionDelta => MainViewMousePosition - s_lastMainViewMousePosition;
    
    public static TimeSpan DeltaTime { get; private set; }
    public static double DeltaTimeInSeconds => DeltaTime.TotalSeconds;
    public static int DeltaTimeInMilliseconds => DeltaTime.Milliseconds;
    
    
    // Elements are ordered based on their priority
    public static List<Element> Elements { get; private set; }
    

    static App()
    {
        s_lastMousePosition = new();
        s_lastElementViewMousePosition = new();
        s_lastMainViewMousePosition = new();
        
        RenderMode = RenderMode.Pinned;
        
        MousePosition = new();
        ElementViewMousePosition = new();
        MainViewMousePosition = new();
        
        s_deltaTimeStopwatch = new();

        _mouseInputWasCaught = false;
        
        DeltaTime = TimeSpan.Zero;
        
        Elements = [];
        
        // workaround for enabling OpenTK (OpenGL Context) integration with SFML.
        // this should be ALWAYS initialized before the window
        _ = new GameWindow(new(), new() { StartVisible = false });
    }


    public static void Init(Font defaultFont)
    {
        if (s_initialized)
        {
            Console.WriteLine("App has already been initialized.");
            return;
        }
        
        TextElement.DefaultTextFont = defaultFont;
        
        s_deltaTimeStopwatch.Start();

        s_initialized = true;
    }


    public static void Init(VideoMode mode, string title, Font defaultFont, Styles style = Styles.Default, ContextSettings settings = new())
    {
        Init(defaultFont);
        InitWindow(new(mode, title, style, settings));
    }
    

    public static void InitWindow(Window window)
    {
        Window = window;
        Window.Resized += (_, args) => OnWindowResize(new(args.Width, args.Height));

        MainView = new(Window.GetView());
        ElementView = new(MainView);
    }
    

    public static void Update()
    {
        ThrowAppNotInitializedExceptionIfNotInitialized();
        
        Window.Update();
        
        SortElementListByPriority();
        
        UpdateMousePositionProperties();
        
        SetElementRenderView();
        
        UpdateDeltaTime();
        
        UpdateElementsMouseInputCatch();
        UpdateElements();
        
        UnsetElementRenderView();
    }

    private static void UpdateElements()
    {
        // use Elements.ToList() to avoid: InvalidOperationException "Collection was modified".
        // don't need to use it with DrawElements(), since it SHOULD not modify the element list.
        
        foreach (Element element in Elements.ToList())
            if (element.Visible)
                element.Update();
    }

    private static void UpdateElementsMouseInputCatch()
    {
        _mouseInputWasCaught = false;
        
        for (int i = Elements.Count - 1; i >= 0; i--)
        {
            Element element = Elements[i];
            IClickable? clickable = element as IClickable;
            
            bool isMouseOver = clickable?.IsPointOver(ElementViewMousePosition) ?? element.IsPointOverBounds(ElementViewMousePosition);

            if (clickable is not null)
                clickable.MouseState.IsMouseInputCaught = !_mouseInputWasCaught && isMouseOver;

            if (element.Visible && element.BlocksMouseInput && isMouseOver && !_mouseInputWasCaught)
                _mouseInputWasCaught = true;
        }
    }

    private static void UpdateMousePositionProperties()
    {
        s_lastMousePosition = MousePosition;
        s_lastElementViewMousePosition = ElementViewMousePosition;
        s_lastMainViewMousePosition = MainViewMousePosition;

        MousePosition = Window.MousePosition;
        ElementViewMousePosition = Window.MapPixelToCoords(MousePosition, ElementView);
        MainViewMousePosition = Window.MapPixelToCoords(MousePosition, MainView);
    }

    private static void UpdateDeltaTime()
    {
        DeltaTime = s_deltaTimeStopwatch.Elapsed;
        s_deltaTimeStopwatch.Restart();
    }
    
    
    private static void SortElementListByPriority()
        => Elements = (from element in Elements
                        orderby element.Priority
                        select element).ToList();


    public static void Draw()
    {
        ThrowAppNotInitializedExceptionIfNotInitialized();
        
        Window.Draw();
        
        SetElementRenderView();
        
        DrawElements();
        
        UnsetElementRenderView();
    }

    private static void DrawElements()
    {
        foreach (Element element in Elements)
            if (element.Visible)
                element.Draw(Window);
    }
    
    
    private static void SetElementRenderView()
        => Window.SetView(ElementView);
    
    private static void UnsetElementRenderView()
        => Window.SetView(MainView);


    public static void AddElement(Element element)
    {
        AddSingleElement(element);
        AddElementsHierarchy(element.Children);
    }

    private static void AddElementsHierarchy(List<Element> elements)
    {
        AddSingleElements(elements);
        
        foreach (Element element in elements)
            AddElementsHierarchy(element.Children);
    }

    private static void AddSingleElement(Element element)
    {
        if (!HasElement(element))
            Elements.Add(element);
    }
    
    private static void AddSingleElements(IEnumerable<Element> elements)
    {
        foreach (Element element in elements)
            AddSingleElement(element);
    }


    public static void RemoveElement(Element element)
    {
        if (!Elements.Remove(element))
            return;

        for (int i = 0; i < Elements.Count; i++)
        {
            Element el = Elements[i];

            if (!el.IsChildOf(element))
                continue;
            
            Elements.Remove(el);
            i--;
        }
    }


    public static bool HasElement(Element element)
        => Elements.Contains(element); 


    public static bool IsMouseOverAnyElement() => _mouseInputWasCaught;

    
    private static void OnWindowResize(Vec2u newSize)
    {
        MainView.Size = newSize;
        
        ElementView.Size = newSize;
        ElementView.Center = (Vector2f)newSize / 2f;
    }


    private static void ThrowAppNotInitializedExceptionIfNotInitialized()
    {
        if (!s_initialized)
            throw AppNotInitializedException();
    }
    
    
    private static InvalidOperationException AppNotInitializedException()
        => new("App not initialized.");
}