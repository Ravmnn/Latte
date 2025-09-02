using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using OpenTK.Windowing.Desktop;

using SFML.Graphics;
using SFML.System;
using SFML.Window;

using Latte.Core.Type;
using Latte.Application.Elements.Primitives;
using Latte.Exceptions.Application;
using Latte.Tweening;
using static SFML.Window.Cursor;

using Debugger = Latte.Debugging.Debugger;


namespace Latte.Application;


// TODO: add time-based performance analyzer, with FPS calculator

// TODO: add text inputs
// TODO: add dropdown
// TODO: add radial buttons
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

    private static readonly Stopwatch s_deltaTimeStopwatch;

    private static bool s_elementWasAddedAndNotUpdated;

    private static readonly List<TweenAnimation> s_tweenAnimations;


    public static Debugger? Debugger { get; private set; }

    public static Font DefaultFont
    {
        get => s_defaultFont ?? throw new AppNotInitializedException();
        set => s_defaultFont = value;
    }

    public static Window Window
    {
        get => s_window ?? throw new AppNotInitializedException();
        private set => s_window = value;
    }

    public static bool ShouldQuit { get; private set; }

    public static View MainView
    {
        get => s_mainView ?? throw new AppNotInitializedException();
        private set => s_mainView = value;
    }

    public static View ElementView
    {
        get
        {
            if (RenderMode == RenderMode.Unpinned)
                return MainView;

            return s_elementView ?? throw new AppNotInitializedException();
        }

        private set => s_elementView = value;
    }

    public static RenderMode RenderMode { get; set; }

    public static Section Section { get; set; }
    public static IEnumerable<Element> Elements => Section.Elements;

    public static TimeSpan DeltaTime { get; private set; }
    public static double DeltaTimeInSeconds => DeltaTime.TotalSeconds;
    public static int DeltaTimeInMilliseconds => DeltaTime.Milliseconds;

    public static ColorRGBA BackgroundColor { get; set; }
    public static bool ManualClearDisplayProcess { get; set; }

    public static bool HasInitialized { get; private set; }


    static App()
    {
        HasInitialized = false;
        s_deltaTimeStopwatch = new Stopwatch();
        s_elementWasAddedAndNotUpdated = false;

        s_tweenAnimations = [];

        RenderMode = RenderMode.Pinned;

        Section = new Section();
        Section.ElementAddedEvent += (_, _) => OnSectionElementAdded();

        DeltaTime = TimeSpan.Zero;

        BackgroundColor = Color.Black;
        ManualClearDisplayProcess = false;

        // workaround for enabling OpenTK (OpenGL Context) integration with SFML.
        // this must be ALWAYS initialized before the rendering window
        _ = new GameWindow(new GameWindowSettings(), new NativeWindowSettings { StartVisible = false });
    }


    public static void Init(Font defaultFont)
    {
        if (HasInitialized)
        {
            Console.WriteLine("App has already been initialized.");
            return;
        }

        DefaultFont = defaultFont;

        Debugger = new Debugger();

        s_deltaTimeStopwatch.Start();

        HasInitialized = true;
    }


    public static void Init(VideoMode mode, string title, Font defaultFont, Styles style = Styles.Default, ContextSettings? settings = null)
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
        KeyboardInput.AddKeyListeners(Window);

        // TODO: add deinit method

        MainView = new View(Window.GetView());
        ElementView = new View(MainView);
    }


    public static void Quit() => ShouldQuit = true;


    public static void Update()
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();

        UpdateDeltaTime();

        Window.Update();
        SetCursorToDefault();


        SetElementRenderView();

        // mouse input needs correct mouse coordinate information, so
        // it needs to update while using the correct view.
        MouseInput.Update();
        NavigationManager.Update();
        FocusManager.Update();

        Section.Update();
        Debugger?.Update(); // update before elements

        UpdateTweenAnimations();
        UpdateElementsAndCheckForNewElements();

        UnsetElementRenderView();


        KeyboardInput.ClearKeyBuffers();
    }

    private static void UpdateDeltaTime()
    {
        DeltaTime = s_deltaTimeStopwatch.Elapsed;
        s_deltaTimeStopwatch.Restart();
    }

    private static void UpdateTweenAnimations()
    {
        foreach (var animation in s_tweenAnimations.ToArray())
            animation.Update();
    }

    private static void SetCursorToDefault()
    {
        Window.Cursor.Type = CursorType.Arrow;
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
            if (element.Active && !constantUpdateOnly)
                element.Update();

            element.ConstantUpdate();
        }
    }


    public static void Draw()
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();

        Window.Draw();

        SetElementRenderView();

        if (!ManualClearDisplayProcess)
            Window.Clear(BackgroundColor);

        Section.Draw(Window);
        DrawElements();

        Debugger?.Draw(Window); // draw after elements

        if (!ManualClearDisplayProcess)
            Window.Display();

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


    public static void AddElements(params IEnumerable<Element> elements) => Section.AddElements(elements);
    public static void AddElement(Element element) => Section.AddElement(element);
    public static void RemoveElements(params IEnumerable<Element> elements) => Section.RemoveElements(elements);
    public static bool RemoveElement(Element element) => Section.RemoveElement(element);
    public static bool HasElement(Element element) => Section.HasElement(element);


    // TODO: maybe move this logic to something like AnimationManager
    public static TweenAnimation AddTweenAnimation(TweenAnimation animation)
    {
        if (s_tweenAnimations.Contains(animation))
            return animation;

        animation.AbortEvent += OnAnimationEnd;
        animation.FinishEvent += OnAnimationEnd;

        s_tweenAnimations.Add(animation);

        return animation;
    }

    public static TweenAnimation RemoveTweenAnimation(TweenAnimation animation)
    {
        s_tweenAnimations.Remove(animation);

        animation.AbortEvent -= OnAnimationEnd;
        animation.FinishEvent -= OnAnimationEnd;

        return animation;
    }


    private static void OnAnimationEnd(object? sender, EventArgs _)
    {
        if (sender is not TweenAnimation animation)
            return;

        RemoveTweenAnimation(animation);
    }


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
}
