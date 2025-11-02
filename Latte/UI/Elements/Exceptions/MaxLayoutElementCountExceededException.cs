using System;

using Latte.Exceptions;


namespace Latte.UI.Elements.Exceptions;




public class MaxLayoutElementCountExceededException : LatteException
{
    private const string MessageLiteral = "The max number of elements has been exceeded.";




    public MaxLayoutElementCountExceededException() : base(MessageLiteral)
    {
    }

    public MaxLayoutElementCountExceededException(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
