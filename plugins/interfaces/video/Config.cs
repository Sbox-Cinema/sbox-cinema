using Sandbox;

namespace CinemaTeam.Plugins.Video;

public static class Config
{
    [ConVar.Client("ctmi.audio.volume")]
    public static float DefaultMediaVolume { get; set; } = 1f;
}
