using System;
using System.Diagnostics;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Primitives;


using Debugger = Latte.Core.Application.Debugging.Debugger;


namespace Latte.Core.Application;


// TODO: add text inputs
// TODO: add effects, which includes blur (a shader maybe), shadow and gradient (shader)


public enum RenderMode
{
    Pinned,
    Unpinned
}


public static class App
{
    private static Font? s_defaultFont;

    private static Window? s_window;
    private static View? s_mainView;
    private static View? s_elementView;

    private static bool s_initialized;

    private static Vec2i s_lastMousePosition;
    private static Vec2f s_lastElementViewMousePosition;
    private static Vec2f s_lastMainViewMousePosition;

    private static readonly Stopwatch s_deltaTimeStopwatch;


    public static Debugger Debugger { get; private set; }

    public static Font DefaultFont
    {
        get => s_defaultFont ?? throw new InvalidOperationException("Default font is not defined.");
        set => s_defaultFont = value;
    }

    public static Window Window
    {
        get => s_window ?? throw AppNotInitializedException();
        private set => s_window = value;
    }

    public static bool ShouldQuit { get; private set; }

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

    public static Section Section { get; set; }
    public static Element[] Elements => Section.Elements;

    public static Element? ElementWhichCaughtMouseInput { get; private set; }
    public static Element? TrueElementWhichCaughtMouseInput { get; private set; }

    public static KeyEventArgs? PressedKey { get; private set; }
    public static KeyEventArgs? ReleasedKey { get; private set; }

    public static Vec2i MousePosition { get; private set; }
    public static Vec2i MousePositionDelta => MousePosition - s_lastMousePosition;
    public static Vec2f ElementViewMousePosition { get; private set; }
    public static Vec2f MainViewMousePosition { get; private set; }
    public static Vec2f ElementViewMousePositionDelta => ElementViewMousePosition - s_lastElementViewMousePosition;
    public static Vec2f MainViewMousePositionDelta => MainViewMousePosition - s_lastMainViewMousePosition;

    public static float MouseScrollDelta { get; private set; }

    public static TimeSpan DeltaTime { get; private set; }
    public static double DeltaTimeInSeconds => DeltaTime.TotalSeconds;
    public static int DeltaTimeInMilliseconds => DeltaTime.Milliseconds;


    static App()
    {
        s_initialized = false;

        s_lastMousePosition = new();
        s_lastElementViewMousePosition = new();
        s_lastMainViewMousePosition = new();

        s_deltaTimeStopwatch = new();

        Debugger = new();

        RenderMode = RenderMode.Pinned;

        Section = new();

        MousePosition = new();
        ElementViewMousePosition = new();
        MainViewMousePosition = new();

        DeltaTime = TimeSpan.Zero;

        // workaround for enabling OpenTK (OpenGL Context) integration with SFML.
        // this must be ALWAYS initialized before the rendering window
        _ = new GameWindow(new(), new() { StartVisible = false });
    }


    public static void Init(Font defaultFont)
    {
        if (s_initialized)
        {
            Console.WriteLine("App has already been initialized.");
            return;
        }

        DefaultFont = defaultFont;

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
        Window.Closed += (_, _) => Quit();
        Window.Resized += (_, args) => OnWindowResize(new(args.Width, args.Height));
        Window.MouseWheelScrolled += (_, args) => MouseScrollDelta = args.Delta;

        Window.KeyPressed += (_, args) => PressedKey = args;
        Window.KeyReleased += (_, args) => ReleasedKey = args;

        MainView = new(Window.GetView());
        ElementView = new(MainView);
    }


    public static void Quit() => ShouldQuit = true;


    public static void Update()
    {
        ThrowAppNotInitializedExceptionIfNotInitialized();

        Window.Update();

        UpdateMouseProperties();
        UpdateDeltaTime();

        SetElementRenderView();

        Section.Update();

        UpdateElementsMouseInputCatch();
        UpdateElements();

        Debugger.Update();

        UnsetElementRenderView();

        MouseScrollDelta = 0f;

        PressedKey = null;
        ReleasedKey = null;
    }

    private static void UpdateMouseProperties()
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

    private static void UpdateElementsMouseInputCatch()
    {
        ElementWhichCaughtMouseInput = TrueElementWhichCaughtMouseInput = null;

        for (int i = Elements.Length - 1; i >= 0; i--)
        {
            Element element = Elements[i];
            IClickable? clickable = element as IClickable;

            bool isMouseOver = clickable?.IsPointOver(ElementViewMousePosition) ?? element.IsPointOverBounds(ElementViewMousePosition);

            if (clickable is not null)
                clickable.CaughtMouseInput = ElementWhichCaughtMouseInput is null && isMouseOver;

            if (!element.Visible || !isMouseOver || ElementWhichCaughtMouseInput is not null)
                continue;

            if (!element.IgnoreMouseInput)
                ElementWhichCaughtMouseInput = element;

            TrueElementWhichCaughtMouseInput ??= element;
        }
    }

    private static void UpdateElements()
    {
        // use ToList() to avoid: InvalidOperationException "Collection was modified".
        // don't need to use it with DrawElements(), since it SHOULD not modify the element list
        // and SHOULD be used only for drawing stuff

        foreach (Element element in Elements)
            if (element.Visible)
                element.Update();
    }


    public static void Draw()
    {
        ThrowAppNotInitializedExceptionIfNotInitialized();

        Window.Draw();

        SetElementRenderView();

        Section.Draw(Window);
        DrawElements();

        Debugger.Draw(Window);

        UnsetElementRenderView();
    }

    private static void DrawElements()
    {
        foreach (Element element in Elements)
            if (element.CanDraw)
                element.Draw(Window);
    }

    private static void SetElementRenderView()
        => Window.SetView(ElementView);

    private static void UnsetElementRenderView()
        => Window.SetView(MainView);


    public static void AddElement(Element element) => Section.AddElement(element);
    public static bool RemoveElement(Element element) => Section.RemoveElement(element);
    public static bool HasElement(Element element) => Section.HasElement(element);


    public static bool IsMouseOverAnyElement() => ElementWhichCaughtMouseInput is not null;


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
