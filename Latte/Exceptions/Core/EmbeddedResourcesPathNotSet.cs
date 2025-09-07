using System;


namespace Latte.Exceptions.Core;


public class EmbeddedResourcesPathNotSet : LatteException
{
    public EmbeddedResourcesPathNotSet() : base("Resources path not set.")
    {
    }

    public EmbeddedResourcesPathNotSet(Exception inner) : base("Resources path not set.", inner)
    {
    }
}
