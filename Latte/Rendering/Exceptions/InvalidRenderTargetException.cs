using System;

using Latte.Exceptions;


namespace Latte.Rendering.Exceptions;


public class InvalidRenderTargetException : LatteException
{
    private const string MessageLiteral =
        "Invalid render target. The interface RenderTarget should not be implemented by custom types.";




    public InvalidRenderTargetException() : base(MessageLiteral)
    {
    }

    public InvalidRenderTargetException(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
