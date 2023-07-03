using Sandbox;
using Sandbox.UI;

namespace Cinema.UI;

public partial class Crosshair : Panel
{
    public Crosshair()
    {
        StyleSheet.Load("/ui/hud/crosshair/Crosshair.scss");
    }

    public override void Tick()
    {
        base.Tick();

        if (Game.LocalPawn is not Player player)
            return;

        var zone = player.GetCurrentTheaterZone();
        if (zone == null || zone.ProjectorEntity == null)
        {
            SetClass("visible", true);
            return;
        }
        var dirToScreen = (zone.ProjectorEntity.ScreenPosition - player.Position).Normal;
        var angleToScreen = player.AimRay.Forward.Dot(dirToScreen);
        SetClass("visible", angleToScreen < 0.6f);
    }
}
