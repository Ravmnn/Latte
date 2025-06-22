using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using SFML.System;
using SFML.Window;
using SFML.Graphics;

using OpenTK.Windowing.Desktop;

using Latte.Core.Type;
using Latte.Elements.Primitives;


using Debugger = Latte.Core.Application.Debugging.Debugger;


namespace Latte.Core.Application;


// TODO: add text inputs
// TODO: add effects, which includes blur (a shader maybe), shadow and gradient (shader)
// TODO: add the default widget library, Vanilla. (the elements in here should be only the main functionality)

// TODO: use custom exceptions whenever convenient.
// TODO: add the ability to choose whether or not Latte should manage the clear/display process automatically.


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

    private static readonly Stopwatch s_deltaTimeStopwatch;

    private static bool s_elementWasAddedAndNotUpdated;


    public static Debugger? Debugger { get; private set; }

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
    public static IEnumerable<Element> Elements => Section.Elements;

    public static KeyEventArgs? PressedKey { get; private set; }
    public static KeyEventArgs? ReleasedKey { get; private set; }

    public static TimeSpan DeltaTime { get; private set; }
    public static double DeltaTimeInSeconds => DeltaTime.TotalSeconds;
    public static int DeltaTimeInMilliseconds => DeltaTime.Milliseconds;


    static App()
    {
        s_initialized = false;
        s_deltaTimeStopwatch = new Stopwatch();
        s_elementWasAddedAndNotUpdated = false;

        RenderMode = RenderMode.Pinned;

        Section = new Section();
        Section.ElementAddedEvent += (_, _) => OnSectionElementAdded();

        DeltaTime = TimeSpan.Zero;

        // workaround for enabling OpenTK (OpenGL Context) integration with SFML.
        // this must be ALWAYS initialized before the rendering window
        _ = new GameWindow(new GameWindowSettings(), new NativeWindowSettings { StartVisible = false });
    }


    public static void Init(Font defaultFont)
    {
        if (s_initialized)
        {
            Console.WriteLine("App has already been initialized.");
            return;
        }

        DefaultFont = defaultFont;

        Debugger = new Debugger();

        s_deltaTimeStopwatch.Start();

        s_initialized = true;
    }


    public static void Init(VideoMode mode, string title, Font defaultFont, Styles style = Styles.Default, ContextSettings settings = new ContextSettings())
    {
        Init(defaultFont);
        InitWindow(new Window(mode, title, style, settings));
    }


    public static void InitWindow(Window window)
    {
        Window = window;
        Window.Closed += (_, _) => Quit();
        Window.Resized += (_, args) => OnWindowResize(new Vec2u(args.Width, args.Height));

        MouseInput.AddScrollListener(Window);

        Window.KeyPressed += (_, args) => PressedKey = args;
        Window.KeyReleased += (_, args) => ReleasedKey = args;

        MainView = new View(Window.GetView());
        ElementView = new View(MainView);
    }


    public static void Quit() => ShouldQuit = true;


    public static void Update()
    {
        ThrowAppNotInitializedExceptionIfNotInitialized();

        UpdateDeltaTime();
        MouseInput.Update();

        Window.Update();

        SetElementRenderView();

        Section.Update();
        Debugger?.Update(); // update before elements

        UpdateElementsAndCheckForNewElements();

        UnsetElementRenderView();

        PressedKey = null;
        ReleasedKey = null;
    }

    private static void UpdateDeltaTime()
    {
        DeltaTime = s_deltaTimeStopwatch.Elapsed;
        s_deltaTimeStopwatch.Restart();
    }

    private static void UpdateElementsAndCheckForNewElements()
    {
        UpdateElements();

        // if an element is added inside an Element.Update method, it won't be updated.
        // to avoid bugs due to it, whenever an element is added inside an Element.Update method,
        // another Update call will be made to update new added elements.

        while (s_elementWasAddedAndNotUpdated)
        {
            s_elementWasAddedAndNotUpdated = false;
            UpdateElements(true);
        }
    }

    private static void UpdateElements(bool constantUpdateOnly = false)
    {
        // use ToArray() to avoid: InvalidOperationException "Collection was modified".
        // don't need to use it with DrawElements(), since it SHOULD not modify the element list
        // and SHOULD be used only for drawing stuff

        foreach (var element in Elements.ToArray())
        {
            if (element.Visible && !constantUpdateOnly)
                element.Update();

            element.ConstantUpdate();
        }
    }


    public static void Draw()
    {
        ThrowAppNotInitializedExceptionIfNotInitialized();

        Window.Draw();

        SetElementRenderView();

        Section.Draw(Window);
        DrawElements();

        Debugger?.Draw(Window); // draw after elements

        UnsetElementRenderView();
    }

    private static void DrawElements()
    {
        foreach (var element in Elements)
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


    private static void OnWindowResize(Vec2u newSize)
    {
        MainView.Size = newSize;

        ElementView.Size = newSize;
        ElementView.Center = (Vector2f)newSize / 2f;
    }


    private static void OnSectionElementAdded()
    {
        s_elementWasAddedAndNotUpdated = true;
    }


    private static void ThrowAppNotInitializedExceptionIfNotInitialized()
    {
        if (!s_initialized)
            throw AppNotInitializedException();
    }


    private static InvalidOperationException AppNotInitializedException() => new InvalidOperationException("App not initialized.");
}
