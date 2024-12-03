
using System;

using SFML.Graphics;


namespace Latte.Elements;


[Flags]
public enum SizePolicyType
{
    None = 0,
    
    FitParentHorizontally = 1 << 0,
    FitParentVertically = 1 << 1,
    FitParent = FitParentHorizontally | FitParentVertically,
}


public interface ISizePoliciable
{
    FloatRect GetSizePolicyRect(SizePolicyType policyType);
}


public static class SizePolicyCalculator
{
    public static FloatRect GetRectOfChild(FloatRect child, FloatRect parent, SizePolicyType policyType)
    {
        FloatRect rect = child;

        if (policyType.HasFlag(SizePolicyType.FitParentHorizontally))
        {
            rect.Left = parent.Left;
            rect.Width = parent.Width;
        }

        if (policyType.HasFlag(SizePolicyType.FitParentVertically))
        {
            rect.Top = parent.Top;
            rect.Height = parent.Height;
        }
        
        return rect;
    }
}