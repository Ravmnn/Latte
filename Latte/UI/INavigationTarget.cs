namespace Latte.UI;


public interface INavigationTarget : IKeyboardInputTarget
{
    int NavigationPriority { get; set; }
}
