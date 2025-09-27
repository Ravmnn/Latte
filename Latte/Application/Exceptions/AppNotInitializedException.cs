using System;

using Latte.Exceptions;


namespace Latte.Application.Exceptions;




public class AppNotInitializedException : LatteException
{
    private const string MessageLiteral = "App not initialized.";




    public AppNotInitializedException() : base(MessageLiteral)
    {
    }

    public AppNotInitializedException(Exception inner) : base(MessageLiteral, inner)
    {
    }




    public static void ThrowIfAppWasNotInitialized()
    {
        if (!App.HasInitialized)
            throw new AppNotInitializedException();
    }
}
