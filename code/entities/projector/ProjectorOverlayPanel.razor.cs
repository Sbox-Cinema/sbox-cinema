using CinemaTeam.Plugins.Media;
using Sandbox.UI;
using System;

namespace Cinema;

public partial class ProjectorOverlayPanel : WorldPanel
{
    public IMediaPlayer Media;
    public bool IsPaused => Media?.Controls.IsPaused ?? false;
    public bool IsBuffering => !(Media?.MediaLoaded ?? true);
    public int BufferIconRotation { get; set; } = 359;

    protected override int BuildHash()
    {
        return HashCode.Combine(IsPaused, IsBuffering);
    }

    public override void Tick()
    {
        base.Tick();

        if (IsBuffering)
        {
            BufferIconRotation--;
            BufferIconRotation %= 360;
            StateHasChanged();
        }
    }
}
