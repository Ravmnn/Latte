using System;

using Latte.Application;


namespace Latte.Exceptions.Application;


public class AppNotInitializedException : LatteException
{
    public AppNotInitializedException() : base("App not initialized.")
    {
    }

    public AppNotInitializedException(Exception inner) : base("App not initialized.", inner)
    {
    }


    public static void ThrowIfAppWasNotInitialized()
    {
        if (!App.HasInitialized)
            throw new AppNotInitializedException();
    }
}
