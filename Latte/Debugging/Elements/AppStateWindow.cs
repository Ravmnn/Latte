using System.Linq;
using System.Text;

using Latte.Core.Type;
using Latte.Application;
using Latte.UI;
using Latte.UI.Elements;


namespace Latte.Debugging.Elements;




[DebuggerIgnoreInspection]
public class AppStateWindow : DebugWindow
{
    private int _lastElementCount;




    public DebugScrollArea ScrollArea { get; }
    public TextElement State { get; }




    public AppStateWindow() : base("App State", new Vec2f(500, 10), new Vec2f(300, 300))
    {
        ScrollArea = new DebugScrollArea(this, null, new Vec2f())
        {
            SizePolicy = SizePolicy.FitParent,
            SizePolicyMargin = new Vec2f(10, 30),

            AlignmentMargin = new Vec2f(0, 20)
        };

        State = new TextElement(ScrollArea, new Vec2f(10, 10), 13, "");
    }




    public override void Update()
    {
        var elementCount = App.Objects.Count();

        if (_lastElementCount != elementCount)
            State.Text = $"{App.Objects.Count()}\n\n{GetElementsText()}";

        _lastElementCount = elementCount;

        base.Update();
    }




    private string GetElementsText()
    {
        var builder = new StringBuilder();

        foreach (var element in App.Objects)
            builder.AppendLine($"{element.GetType().Name} - {element.Priority}");

        return builder.ToString();
    }
}
