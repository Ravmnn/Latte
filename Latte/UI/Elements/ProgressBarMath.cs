using System;


namespace Latte.UI.Elements;




public static class ProgressBarMath
{
    public static float CalculateNormalizedProgress(float value, float min, float max)
        => (value - min) / Math.Max(1, max - min);
}
