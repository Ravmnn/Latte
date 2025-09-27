using System;

using Latte.Exceptions;


namespace Latte.UI.Elements.Exceptions;




public class EmptyGridException : LatteException
{
    private const string MessageLiteral = "Grid is empty.";




    public EmptyGridException() : base(MessageLiteral)
    {
    }

    public EmptyGridException(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
