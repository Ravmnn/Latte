using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;

using OpenTK.Windowing.Desktop;

using SFML.System;
using SFML.Graphics;
using SFML.Window;

using Latte.Core;
using Latte.Core.Type;
using Latte.UI.Elements;
using Latte.Exceptions.Application;
using Latte.Tweening;


using static SFML.Window.Cursor;


using Debugger = Latte.Debugging.Debugger;
using VideoMode = SFML.Window.VideoMode;


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
    private static View? s_objectView;

    private static readonly Stopwatch s_deltaTimeStopwatch;

    private static bool s_objectWasAddedAndNotUpdated;

    private static readonly List<TweenAnimation> s_tweenAnimations; // TODO: move to somewhere like AnimationManager


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

    // TODO: this view system is probably no needed anymore, consider removing

    public static View MainView
    {
        get => s_mainView ?? throw new AppNotInitializedException();
        private set => s_mainView = value;
    }

    public static View ObjectView
    {
        get
        {
            if (RenderMode == RenderMode.Unpinned)
                return MainView;

            return s_objectView ?? throw new AppNotInitializedException();
        }

        private set => s_objectView = value;
    }

    public static RenderMode RenderMode { get; set; }

    public static Section Section { get; set; }
    public static IEnumerable<BaseObject> Objects => Section.Objects;

    public static TimeSpan DeltaTime { get; private set; }
    public static double DeltaTimeInSeconds => DeltaTime.TotalSeconds;
    public static int DeltaTimeInMilliseconds => DeltaTime.Milliseconds;

    public static ColorRGBA BackgroundColor { get; set; }
    public static bool ManualClearDisplayProcess { get; set; }

    public static bool HasInitialized { get; private set; }


    static App()
    {
        s_deltaTimeStopwatch = new Stopwatch();
        s_objectWasAddedAndNotUpdated = false;

        s_tweenAnimations = [];


        HasInitialized = false;

        RenderMode = RenderMode.Pinned;

        Section = new Section();
        Section.ObjectAddedEvent += (_, _) => OnSectionElementAdded();

        DeltaTime = TimeSpan.Zero;

        BackgroundColor = Color.Black;
        ManualClearDisplayProcess = false;

        // TODO: not every Latte application may use a window, so improve this

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
        ObjectView = new View(MainView);
    }


    public static void Quit() => ShouldQuit = true;


    public static void Update()
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();

        UpdateDeltaTime();

        Window.Update();
        SetCursorToDefault();


        SetObjectRenderView();

        // mouse input needs correct mouse coordinate information, so
        // it needs to update while using the correct view.
        MouseInput.Update();
        NavigationManager.Update();
        FocusManager.Update();

        Section.Update();
        Debugger?.Update(); // update before elements

        UnsetObjectRenderView();
        UpdateTweenAnimations();
        UpdateObjectsAndCheckForNewOnes();



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

    private static void UpdateObjectsAndCheckForNewOnes()
    {
        UpdateObjects();

        // if an element is added inside an Element.Update method, it won't be updated.
        // to avoid bugs due to it, whenever an element is added inside an Element.Update method,
        // another Update call will be made to update new added elements.

        while (s_objectWasAddedAndNotUpdated)
        {
            s_objectWasAddedAndNotUpdated = false;
            UpdateObjects(true);
        }
    }

    private static void UpdateObjects(bool constantUpdateOnly = false)
    {
        // use ToArray() to avoid: InvalidOperationException "Collection was modified".
        // don't need to use it with DrawElements(), since it SHOULD not modify the element list
        // and SHOULD be used only for drawing stuff

        foreach (var @object in Objects.ToArray())
        {
            if (@object.CanUpdate && !constantUpdateOnly)
                @object.Update();

            @object.ConstantUpdate();
        }
    }


    public static void Draw(IRenderer renderer)
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();


        SetObjectRenderView();

        Section.Draw(renderer);
        DrawObjects(renderer);

        Debugger?.Draw(renderer); // draw after elements

        UnsetObjectRenderView();
    }


    public static void Draw()
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();

        if (!ManualClearDisplayProcess)
            Window.Clear(BackgroundColor);

        Draw(Window.Renderer);

        if (!ManualClearDisplayProcess)
            Window.Display();
    }

    private static void DrawObjects(IRenderer renderer)
    {
        foreach (var element in Objects)
            if (element.CanDraw)
                element.Draw(renderer);
    }

    private static void SetObjectRenderView()
        => Window.SetView(ObjectView);

    private static void UnsetObjectRenderView()
        => Window.SetView(MainView);


    public static void AddObjects(params IEnumerable<BaseObject> objects) => Section.AddObjects(objects);
    public static void AddObject(BaseObject @object) => Section.AddObject(@object);
    public static void RemoveObjects(params IEnumerable<BaseObject> objects) => Section.RemoveObjects(objects);
    public static bool RemoveObject(BaseObject @object) => Section.RemoveObject(@object);
    public static bool HasObject(BaseObject @object) => Section.HasObject(@object);

    public static void AddElements(params IEnumerable<Element> elements) => Section.AddElements(elements);
    public static void AddElement(Element element) => Section.AddElement(element);
    public static void RemoveElements(params IEnumerable<Element> elements) => Section.RemoveElements(elements);
    public static bool RemoveElement(Element element) => Section.RemoveElement(element);


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

        ObjectView.Size = newSize;
        ObjectView.Center = (Vector2f)newSize / 2f;
    }


    private static void OnSectionElementAdded()
    {
        s_objectWasAddedAndNotUpdated = true;
    }
}
