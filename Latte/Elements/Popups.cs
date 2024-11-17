using Latte.Core;
using Latte.Elements.Primitives;


namespace Latte.Elements;


public class ProgressBarPopup : WindowElement
{
    public ProgressBarElement ProgressBar { get; }
    
    public bool CloseOnComplete { get; set; }
    
    
    public ProgressBarPopup(string title) : base(title, new(), new(300, 200))
    {
        Alignment = AlignmentType.Center;
        
        CloseOnComplete = true;
        
        ProgressBar = new(this, new(), new(Size.X - 30, 20))
        {
            Alignment = AlignmentType.HorizontalCenter | AlignmentType.Bottom,
            AlignmentMargin = new(0, -10)
        };
    }


    public override void Update()
    {
        if (!Visible)
            return;
        
        if (!IsClosed && CloseOnComplete && ProgressBar.Completed)
            Close();
        
        base.Update();
    }
}