using System;
using System.Linq;
using System.Collections.Generic;

using OpenTK.Windowing.Desktop;

using SFML.System;
using SFML.Graphics;
using SFML.Window;

using Latte.Core.Objects;
using Latte.Core.Type;
using Latte.Rendering;
using Latte.UI.Elements;
using Latte.Application.Exceptions;
using Latte.Communication.Bridge;


using static SFML.Window.Cursor;


using Debugger = Latte.Debugging.Debugger;
using VideoMode = SFML.Window.VideoMode;


namespace Latte.Application;


// TODO: add time-based performance analyzer, with FPS calculator

// TODO: add horizontal and vertical layouts
// TODO: add dropdown
// TODO: add radial buttons
// TODO: add effects, which includes blur (a shader maybe), shadow and gradient (shader)




public static class App
{
    private static bool s_objectWasAddedAndNotUpdated;




    public static BridgeNode? Bridge { get; set; }
    public static Debugger? Debugger { get; private set; }


    private static Window? s_window;
    public static Window Window
    {
        get => s_window ?? throw new AppNotInitializedException();
        private set => s_window = value;
    }

    private static Font? s_defaultFont;
    public static Font DefaultFont
    {
        get => s_defaultFont ?? throw new AppNotInitializedException();
        set => s_defaultFont = value;
    }


    public static bool ShouldQuit { get; private set; }
    public static bool HasInitialized { get; private set; }




    private static Section s_section;
    public static Section Section
    {
        get => s_section;
        set
        {
            s_section.Deinitialize();
            s_section = value;
            s_section.Initialize();

            SectionChangedEvent?.Invoke(null, EventArgs.Empty);
        }
    }

    public static IEnumerable<BaseObject> Objects => Section.Objects;


    public static event EventHandler? SectionChangedEvent;




    public static ColorRGBA BackgroundColor { get; set; }
    public static bool ManualClearDisplayProcess { get; set; }
    public static bool ManualObjectUpdate { get; set; }
    public static bool ManualObjectDraw { get; set; }




    static App()
    {
        s_objectWasAddedAndNotUpdated = false;


        HasInitialized = false;

        s_section = new Section();
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
        AppAlreadyInitializedException.ThrowIfAppIsInitialized();


        DefaultFont = defaultFont;
        Debugger = new Debugger();

        DeltaTime.Start();

        HasInitialized = true;
    }


    public static void Init(VideoMode mode, string title, AppInitializationSettings? settings = null)
    {
        settings ??= AppInitializationSettings.Default;


        Init(settings.Value.DefaultFont);
        InitWindow(new Window(mode, title, settings.Value.WindowStyle, settings.Value.ContextSettings));
    }


    public static void InitWindow(Window window)
    {
        Window = window;
        AddEventListeners(Window);
    }


    public static void InitBridge(string processName)
        => Bridge = new BridgeNode(processName);





    public static void Deinit()
    {
        DeinitBridge();
        DeinitWindow();
    }

    public static void DeinitWindow()
    {
        RemoveEventListeners(Window);
        Window.Close();
    }

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
        Debugger?.Update();

        UpdateObjectsAndCheckForNewOnes();
    }


    private static void SetCursorToDefault()
        => Window.Cursor.Type = CursorType.Arrow;


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


    private static void UpdateObjects(bool unconditionalUpdateOnly = false)
    {
        // use ToArray() to avoid: InvalidOperationException "Collection was modified".
        // don't need to use it with DrawElements(), since it SHOULD not modify the element list
        // and SHOULD be used only for drawing stuff

        foreach (var @object in Objects.ToArray())
            UpdateObject(@object, unconditionalUpdateOnly);
    }


    public static void UpdateObject(BaseObject @object, bool unconditionalUpdateOnly = false)
    {
        if (@object.CanUpdate && !unconditionalUpdateOnly)
            @object.Update();

        @object.UnconditionalUpdate();
    }




    public static void Draw(IRenderer renderer)
    {
        AppNotInitializedException.ThrowIfAppWasNotInitialized();


        Section.Draw(renderer);
        DrawObjects(renderer);

        Debugger?.Draw(renderer); // draw after elements


        renderer.ApplyPostEffect();
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
