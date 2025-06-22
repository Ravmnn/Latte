using System;

using Latte.Core.Application;


namespace Latte.Exceptions.Application;


public class AppNotInitializedException : Exception
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
