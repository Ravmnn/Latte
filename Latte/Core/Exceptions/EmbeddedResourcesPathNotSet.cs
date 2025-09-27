using System;

using Latte.Exceptions;


namespace Latte.Core.Exceptions;




public class EmbeddedResourcesPathNotSet : LatteException
{
    private const string MessageLiteral = "Resources path is not set.";




    public EmbeddedResourcesPathNotSet() : base(MessageLiteral)
    {
    }

    public EmbeddedResourcesPathNotSet(Exception inner) : base(MessageLiteral, inner)
    {
    }
}
