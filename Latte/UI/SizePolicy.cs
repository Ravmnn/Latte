using System;

using SFML.Graphics;


namespace Latte.UI;


[Flags]
public enum SizePolicy
{
    None = 0,

    FitParentHorizontally = 1 << 0,
    FitParentVertically = 1 << 1,
    FitParent = FitParentHorizontally | FitParentVertically,
}


public interface ISizePoliciable
{
    FloatRect GetSizePolicyRect(SizePolicy policy);
}


public static class SizePolicyCalculator
{
    public static FloatRect CalculateChildRect(FloatRect child, FloatRect parent, SizePolicy policy)
    {
        var rect = child;

        if (policy.HasFlag(SizePolicy.FitParentHorizontally))
        {
            rect.Left = parent.Left;
            rect.Width = parent.Width;
        }

        if (policy.HasFlag(SizePolicy.FitParentVertically))
        {
            rect.Top = parent.Top;
            rect.Height = parent.Height;
        }

        return rect;
    }
}
