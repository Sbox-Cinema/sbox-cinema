﻿namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Allows for the playback of a video to be controlled.
/// </summary>
public interface IMediaControls
{
    bool IsPaused { get; }
    void Resume();
    void SetPaused(bool paused);
    void Stop();
    void Seek(float time);
}
