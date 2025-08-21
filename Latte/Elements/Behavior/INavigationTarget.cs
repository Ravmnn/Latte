namespace Latte.Elements.Behavior;


public interface INavigationTarget : IKeyboardInputTarget
{
    int NavigationPriority { get; set; }

    // TODO: FocusOnSelect; if true, when selected, gets focused
}
