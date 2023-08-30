namespace Sandbox.Utility;

public static partial class EasingExtensions
{
    public static float LinearArch(float f)
    {
        return (f <= 0.5f) ? Easing.Linear(f * 2.0f) : Easing.Linear((1 - f) * 2.0f);
    }
    public static float EaseArch(float f)
    {
        return (f <= 0.5f) ? Easing.EaseIn(f * 2.0f) : Easing.EaseOut((1 - f) * 2.0f);
    }
    public static float SineEaseArch(float f)
    {
        return (f <= 0.5f) ? Easing.SineEaseIn(f * 2.0f) : Easing.SineEaseOut((1 - f) * 2.0f);
    }
    public static float QuadArch(float f)
    {
        return (f <= 0.5f) ? Easing.QuadraticIn(f * 2.0f) : Easing.QuadraticOut((1 - f) * 2.0f);
    }
}
