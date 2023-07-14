﻿using Sandbox;

namespace CinemaTeam.Plugins.Media;

/// <summary>
/// Provides a texture that can be displayed on a screen.
/// </summary>
public interface IVideoPresenter
{
    // TODO: Add a bool for whether the texture updates or is a still image.
    Texture Texture { get; }
    // TODO: Move this in to a separate interface.
    SoundHandle PlayAudio(IEntity entity);
    void SetVolume(float newVolume);
    void SetPosition(Vector3 newPosition);
}
