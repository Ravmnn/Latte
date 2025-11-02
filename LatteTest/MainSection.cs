using SFML.Graphics;

using Latte.Core.Type;
using Latte.UI.Elements;
using Latte.Application;


namespace Latte.Test;




public sealed class MainSection : Section
{
    public MainSection()
    {
        var window = new WindowElement("Window", new Vec2f(), new Vec2f(600, 300))
        {
            Radius = 10f,

            BorderSize = 0f,
            BorderColor = Color.Magenta
        };


        var layout = new GridLayoutElement(window, null, 4)
        {
            Margin = new Vec2f(10f, 0f)
        };

        CreateRectangleInHorizontalLayout(layout, new Vec2f(30, 30), Color.Magenta);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(20, 10), Color.Red);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(90, 60), Color.Green);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(40, 50), Color.Cyan);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(60, 90), Color.Yellow);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(40, 60), Color.Black);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(30, 65), Color.Red);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(10, 20), Color.Blue);
        CreateRectangleInHorizontalLayout(layout, new Vec2f(40, 30), Color.Green);


        AddElement(window);
    }




    private void CreateRectangleInHorizontalLayout(LayoutElement layout, Vec2f size, ColorRGBA? color = null)
    {
        layout.Push(new RectangleElement(layout, null, size) { Color = color ?? Color.White });
    }


    public override void Update()
    {
        foreach (var @object in App.Objects)
            if (@object.Position == new Vec2f())
                Console.WriteLine(@object);

        base.Update();
    }
}
