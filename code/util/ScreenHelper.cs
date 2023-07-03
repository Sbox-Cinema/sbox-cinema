using Sandbox;
using System;

namespace Cinema;
public static class ScreenHelper
{
    /// <summary>
    /// Returns a screenspace 2D bounding box that tightly wraps the given worldspace
    /// 3D bounding box. The returned bounding box is in fractions of screen size, so its
    /// values may be used in <c>Panel</c> styles as <c>Length.Fraction</c>.
    /// </summary>
    /// <param name="bbox">A worldspace 3D bounding box.</param>
    public static Rect ToFractionalScreenBounds(this BBox bbox)
    {
        float xLow = float.PositiveInfinity;
        float xHigh = float.NegativeInfinity;
        float yLow = float.PositiveInfinity;
        float yHigh = float.NegativeInfinity;

        foreach (var corner in bbox.Corners)
        {
            var screenPos = corner.ToScreen();
            if (screenPos.x < xLow)
                xLow = Math.Clamp(screenPos.x, 0, 1);
            if (screenPos.x > xHigh)
                xHigh = Math.Clamp(screenPos.x, 0, 1);
            if (screenPos.y < yLow)
                yLow = Math.Clamp(screenPos.y, 0, 1);
            if (screenPos.y > yHigh)
                yHigh = Math.Clamp(screenPos.y, 0, 1);
        }
        var width = xHigh - xLow;
        var height = yHigh - yLow;
        return new Rect(xLow, yLow, width, height);
    }
}
