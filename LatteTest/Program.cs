using SFML.Window;

using Latte.Application;


namespace Latte.Test;


class Program
{
    private static void Main()
    {
        App.Init(VideoMode.DesktopMode, "Latte Test");
        App.Debugger!.EnableKeyShortcuts = true;


        App.Section = new MainSection();


        while (!App.ShouldQuit)
        {
            App.Update();
            App.Draw();
        }


        App.Deinit();
    }
}
