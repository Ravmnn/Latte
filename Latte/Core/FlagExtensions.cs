using System;
using System.Linq;
using System.Collections.Generic;


namespace Latte.Core;




public static class FlagExtensions
{
    public static bool HasAnyFlag(this Enum source, params IEnumerable<Enum> flags)
        => flags.Any(source.HasFlag);

    public static bool HasAllFlags(this Enum source, params IEnumerable<Enum> flags)
        => flags.All(source.HasFlag);
}
