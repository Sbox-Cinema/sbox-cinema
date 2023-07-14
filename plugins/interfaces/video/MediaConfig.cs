using Sandbox;
using System;

namespace CinemaTeam.Plugins.Media;

public static class MediaConfig
{
    public static float DefaultMediaVolume
    {
        get => _DefaultMediaVolume;
        set
        {
            var oldValue = _DefaultMediaVolume;
            _DefaultMediaVolume = value;
            if (oldValue != value)
            {
                OnDefaultMediaVolumeChanged(oldValue, value);
            }
            // TODO: Persist this value clientside.
        }
    }

    private static float _DefaultMediaVolume = 1f;

    public static bool ShouldMute 
    {
        get => DefaultMediaVolume == 0f;
        set
        {
            if (value)
            {
                _LastVolume = DefaultMediaVolume;
                DefaultMediaVolume = 0f;
            }
            else
            {
                DefaultMediaVolume = _LastVolume;
            }
        }
    }
    private static bool _ShouldMute = false;
    private static float _LastVolume = _DefaultMediaVolume;

    public static event EventHandler<float> DefaultMediaVolumeChanged;

    private static void OnDefaultMediaVolumeChanged(float oldValue, float newValue)
    {
        DefaultMediaVolumeChanged?.Invoke(null, newValue);
    }
}
