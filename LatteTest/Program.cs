using SFML.Window;
using SFML.Graphics;

using Latte.Core.Application;
using Latte.Core.Type;
using Latte.Elements;
using Latte.Elements.Behavior;
using Latte.Elements.Primitives;
using Latte.Elements.Primitives.Shapes;
using OpenTK.Graphics.OpenGL;
using SFML.System;


namespace Latte.Test;


class Program
{
    private static int s_counter;


    private static void AddButtonToLayout(GridLayoutElement layoutElement)
        => layoutElement.AddElementAtEnd(new ButtonElement(null, new Vec2f(), new Vec2f(30, 30), $"{s_counter++}")
        {
            Alignment = { Value = Alignment.Center }
        });


    private static void Main()
    {
        var font = new Font("resources/Roboto-Regular.ttf");
        font.SetSmooth(false);

        App.Init(VideoMode.DesktopMode, "Latte Test", font, settings: new ContextSettings
        {
            AntialiasingLevel = 8,
            DepthBits = 24,
            StencilBits = 8
        });


        App.Debugger.EnableKeyShortcuts = true;
        App.ManualClearDisplayProcess = true;


        var window = new WindowElement("Window", new Vec2f(), new Vec2f(300, 300))
        {
            Radius = { Value = 10f },

            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.Magenta }
        };

        var rect = new RectangleElement(window, new Vec2f(), new Vec2f(200, 200))
        {
            Color = { Value = Color.Green },
            BorderSize = { Value = 2f },
            BorderColor = { Value = Color.Red },

            Radius = { Value = 4f },

            Alignment = { Value = Alignment.VerticalCenter | Alignment.Left },
            AlignmentMargin = { Value = new Vec2f(-50) }
        };

        var button = new ButtonElement(rect, new Vec2f(), new Vec2f(80, 80), "Button")
        {
            Alignment = { Value = Alignment.TopLeft },
            AlignmentMargin = { Value = new Vec2f(-15, -15) }
        };


        App.AddElement(window);

        var shape = new ConvexShape(4);
        shape.SetPoint(0, new Vector2f(200, 200));
        shape.SetPoint(1, new Vector2f(400, 300));
        shape.SetPoint(2, new Vector2f(300, 500));
        shape.SetPoint(3, new Vector2f(100, 250));

        var rectangle = new RectangleShape(new Vector2f(300, 300));
        rectangle.Position = new Vector2f(220, 220);
        rectangle.FillColor = Color.Red;


        while (!App.ShouldQuit)
        {
            App.Update();


            App.Window.Clear();

            // GL.Enable(EnableCap.StencilTest);
            //
            // GL.StencilFunc(StencilFunction.Always, 1, 0xff);
            // GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Replace);
            //
            // GL.ColorMask(false, false, false, false);
            // GL.DepthMask(false);
            //
            // shape.Draw(App.Window, RenderStates.Default);
            //
            // GL.ColorMask(true, true, true, true);
            // GL.DepthMask(true);
            //
            // GL.StencilFunc(StencilFunction.Equal, 1, 0xff);
            // GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            //
            // rectangle.Draw(App.Window, RenderStates.Default);
            //
            // GL.StencilFunc(StencilFunction.Always, 0, 0xff);
            // GL.StencilOp(StencilOp.Keep, StencilOp.Keep, StencilOp.Keep);
            // GL.Disable(EnableCap.StencilTest);

            App.Draw();

            App.Window.Display();
        }
    }
}
