using System;

using Latte.Exceptions;


namespace Latte.Application.Exceptions;




public class AppAlreadyInitializedException : LatteException
{
    private const string MessageLiteral = "App has already initialized.";




    public AppAlreadyInitializedException() : base(MessageLiteral)
    {
    }

    public AppAlreadyInitializedException(Exception inner) : base(MessageLiteral, inner)
    {
    }




    public static void ThrowIfAppIsInitialized()
    {
        if (App.HasInitialized)
            throw new AppAlreadyInitializedException();
    }
}
