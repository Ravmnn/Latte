namespace Latte.Application.Elements.Behavior;


public interface INavigationTarget : IKeyboardInputTarget
{
    int NavigationPriority { get; set; }
}
