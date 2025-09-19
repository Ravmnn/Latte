using System;
using System.Linq;
using System.Collections.Generic;

using OpenTK.Windowing.Desktop;

using SFML.System;
using SFML.Graphics;
using SFML.Window;

using Latte.Core;
using Latte.Core.Objects;
using Latte.Core.Type;
using Latte.UI.Elements;
using Latte.Application.Exceptions;
using Latte.Communication.BridgeProtocol;


using static SFML.Window.Cursor;


using Debugger = Latte.Debugging.Debugger;
using VideoMode = SFML.Window.VideoMode;


namespace Latte.Application;


// TODO: add time-based performance analyzer, with FPS calculator

// TODO: add text inputs
// TODO: add dropdown
// TODO: add radial buttons
// TODO: add effects, which includes blur (a shader maybe), shadow and gradient (shader)


public static class App
{
    private static Font? s_defaultFont;
    private static Window? s_window;

    private static bool s_objectWasAddedAndNotUpdated;


    public static BridgeNode? Bridge { get; set; }

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

    public static Section Section { get; set; }
    public static IEnumerable<BaseObject> Objects => Section.Objects;


    public static ColorRGBA BackgroundColor { get; set; }
    public static bool ManualClearDisplayProcess { get; set; }
    public static bool ManualObjectUpdate { get; set; }
    public static bool ManualObjectDraw { get; set; }

    public static bool HasInitialized { get; private set; }


    static App()
    {
        s_objectWasAddedAndNotUpdated = false;


        HasInitialized = false;

        Section = new Section();
        Section.ObjectAddedEvent += (_, _) => OnSectionElementAdded();

        BackgroundColor = Color.Black;
        ManualClearDisplayProcess = false;

        // TODO: not every Latte application may use a window, so improve this

        // workaround for enabling OpenTK (OpenGL Context) integration with SFML.
        // this must be ALWAYS initialized before the rendering window
        _ = new GameWindow(new GameWindowSettings(), new NativeWindowSettings { StartVisible = false });
    }


    private static void Init(Font defaultFont)
    {
        // TODO: throw exception instead
        if (HasInitialized)
        {
            Console.WriteLine("App has already been initialized.");
            return;
        }

        DefaultFont = defaultFont;

        Debugger = new Debugger();

        DeltaTime.Start();

        HasInitialized = true;
    }


    // TODO: move font, style and settings to a struct like AppInitializationSettings
    public static void Init(VideoMode mode, string title, Font? defaultFont = null, Styles style = Styles.Default, ContextSettings? settings = null)
    {
        Init(defaultFont ?? EmbeddedResources.DefaultFont());
        InitWindow(new Window(mode, title, style, settings));
    }


    public static void Deinit()
    {
        DeinitBridge();
        DeinitWindow();
    }


    public static void InitWindow(Window window)
    {
        Window = window;
        AddEventListeners(Window);
    }

    public static void DeinitWindow()
    {
        RemoveEventListeners(Window);
        Window.Close();
    }


    public static void InitBridge(string processName)
        => Bridge = new BridgeNode(processName);

    public static void DeinitBridge()
    {
        Bridge?.Dispose();
        Bridge = null;
    }


    public static void Quit()
        => ShouldQuit = true;


    public static void AddEventListeners(Window window)
    {
        window.Closed += OnWindowClose;
        window.Resized += OnWindowResize;

        MouseInput.AddScrollListener(window);
        KeyboardInput.AddKeyListeners(window);
    }

    public static void RemoveEventListeners(Window window)
    {
        window.Closed -= OnWindowClose;
        window.Resized -= OnWindowResize;

        MouseInput.RemoveScrollListener(window);
        KeyboardInput.RemoveKeyListeners(window);
    }


    public static void Update()
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();

        DeltaTime.Update();

        Window.Update();
        SetCursorToDefault();


        // mouse input needs correct mouse coordinate information, so
        // it needs to update while using the correct view.
        MouseInput.Update();
        KeyboardInput.Update();
        NavigationManager.Update();
        FocusManager.Update();
        AnimationManager.Update();

        Section.Update();
        Debugger?.Update(); // update before elements

        UpdateObjectsAndCheckForNewOnes();
    }

    private static void SetCursorToDefault()
    {
        Window.Cursor.Type = CursorType.Arrow;
    }

    private static void UpdateObjectsAndCheckForNewOnes()
    {
        if (ManualObjectUpdate)
            return;

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
            UpdateObject(@object, constantUpdateOnly);
    }


    public static void UpdateObject(BaseObject @object, bool constantUpdateOnly = false)
    {
        if (@object.CanUpdate && !constantUpdateOnly)
            @object.Update();

        @object.ConstantUpdate();
    }


    public static void Draw(IRenderer renderer)
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();

        Section.Draw(renderer);
        DrawObjects(renderer);

        Debugger?.Draw(renderer); // draw after elements
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
        if (ManualObjectDraw)
            return;

        foreach (var @object in Objects)
            DrawObject(renderer, @object);
    }


    public static void DrawObject(IRenderer renderer, BaseObject @object)
    {
        if (@object.CanDraw)
            @object.Draw(renderer);
    }


    public static void AddObjects(params IEnumerable<BaseObject> objects) => Section.AddObjects(objects);
    public static void AddObject(BaseObject @object) => Section.AddObject(@object);
    public static void RemoveObjects(params IEnumerable<BaseObject> objects) => Section.RemoveObjects(objects);
    public static bool RemoveObject(BaseObject @object) => Section.RemoveObject(@object);
    public static bool HasObject(BaseObject @object) => Section.HasObject(@object);

    public static void AddElements(params IEnumerable<Element> elements) => Section.AddElements(elements);
    public static void AddElement(Element element) => Section.AddElement(element);
    public static void RemoveElements(params IEnumerable<Element> elements) => Section.RemoveElements(elements);
    public static bool RemoveElement(Element element) => Section.RemoveElement(element);


    private static void OnWindowClose(object? _, EventArgs __)
        => Quit();

    private static void OnWindowResize(object? _, SizeEventArgs args)
    {
        var oldView = Window.GetView();
        var newView = new View(oldView);

        var newSize = new Vec2u(args.Width, args.Height);

        newView.Center = (Vector2f)newSize / 2f;
        newView.Size = newSize;

        Window.SetView(newView);
    }


    private static void OnSectionElementAdded()
    {
        s_objectWasAddedAndNotUpdated = true;
    }
}
