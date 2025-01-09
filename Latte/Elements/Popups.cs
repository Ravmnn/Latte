using Latte.Elements.Primitives;


namespace Latte.Elements;


public class ProgressBarPopup : WindowElement
{
    public ProgressBarElement ProgressBar { get; }
    
    public bool CloseOnComplete { get; set; }
    
    
    public ProgressBarPopup(string title) : base(title, new(), new(300, 200))
    {
        Alignment.Set(Elements.Alignment.Center);
        
        CloseOnComplete = true;

        ProgressBar = new(this, new(), new(Size.Value.X - 30, 20));
        ProgressBar.Alignment.Set(Elements.Alignment.HorizontalCenter | Elements.Alignment.Bottom);
        ProgressBar.AlignmentMargin.Set(new(0, -10));
    }


    public override void Update()
    {
        if (Visible && CloseOnComplete && ProgressBar.Completed)
            Close();
        
        base.Update();
    }
}
