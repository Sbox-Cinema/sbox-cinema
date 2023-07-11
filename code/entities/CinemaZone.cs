using CinemaTeam.Plugins.Video;
using Editor;
using Sandbox;
using Sandbox.Utility;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

[Library("cinema_zone"), HammerEntity, Solid]
[Title("Cinema Zone"), Category("Cinema")]
public partial class CinemaZone : BaseTrigger
{
    /// <summary>
    /// The projector entity that will play in this area
    /// </summary>
    [Property]
    public EntityTarget Projector { get; set; }
    [BindComponent]
    public MediaController MediaController { get; }
    [BindComponent]
    public MediaQueue MediaQueue { get; }
    [Net]
    public ProjectorEntity ProjectorEntity { get; set; }
    [Net]
    public IList<PointLightEntity> Lights { get; set; }
    [Net]
    public IList<Entity> Speakers { get; set; }
    private List<SoundHandle> ActiveSoundHandles { get; set; } = new();

    public float LightDimmingTime { get; set; } = 5.0f;
    private float MaxLightBrightness { get; set; }
    private float DesiredLightBrightness { get; set; }
    private float CurrentLightBrightness { get; set; }

    /// <summary>
    /// Returns true if this zone has a projector and media controller.
    /// </summary>
    public bool IsTheaterZone => MediaController != null && ProjectorEntity != null;

    public override void Spawn()
    {
        base.Spawn();
        Transmit = TransmitType.Always;
    }

    [GameEvent.Entity.PostSpawn]
    public void OnPostSpawn()
    {
        ProjectorEntity = Projector.GetTarget<ProjectorEntity>();
        Components.Create<MediaController>();
        Components.Create<MediaQueue>();

        MediaController.StartPlaying += (_, _) => SetLightsEnabled(false);
        MediaController.StopPlaying += (_, _) => SetLightsEnabled(true);

        InitializeLights();
        InitializeSpeakers();
    }

    [GameEvent.Tick.Server]
    public void OnServerTick()
    {
        if (CurrentLightBrightness != DesiredLightBrightness)
        {
            var delta = Time.Delta / LightDimmingTime * MaxLightBrightness;
            CurrentLightBrightness = MathX.Approach(CurrentLightBrightness, DesiredLightBrightness, delta);
            foreach(var light in Lights)
            {
                light.Brightness = CurrentLightBrightness;
                // Turn off lights if the brightness is zero.
                light.Enabled = CurrentLightBrightness > 0;
            }
        }
    }

    private void InitializeLights()
    {
        Lights = Children
            .OfType<PointLightEntity>()
            .ToList();
        // TODO: Also fetch spotlights, perhaps even make an interface for lights and adapter
        // for each concrete light type so that we can perform common operations on each.

        // Since movies aren't playing at the start, lights should be enabled at first.
        SetLightsEnabled(true);

        if (Lights.Any())
        {
            MaxLightBrightness = Lights.Sum(l => l.Brightness) / Lights.Count();
            CurrentLightBrightness = MaxLightBrightness;
            DesiredLightBrightness = CurrentLightBrightness;
        }
    }

    private void InitializeSpeakers()
    {
        var speakers = Children
                .Where(c => c.Tags.Has("speaker"));

        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("front") && s.Tags.Has("left")));
        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("front") && s.Tags.Has("right")));
        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("center")));
        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("subwoofer")));
        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("side") && s.Tags.Has("left")));
        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("side") && s.Tags.Has("right")));
        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("rear") && s.Tags.Has("left")));
        Speakers.Add(speakers.FirstOrDefault(s => s.Tags.Has("rear") && s.Tags.Has("right")));
    }

    public void SetLightsEnabled(bool newValue)
    {
        DesiredLightBrightness = newValue ? MaxLightBrightness : 0.0f;
    }

    public bool HasSpeaker(AudioChannel channel)
    {
        return Speakers[(int)channel] != null;
    }

    public Entity GetSpeaker(AudioChannel channel)
    {
        return Speakers[(int)channel];
    }

    public void PlayAudioOnSpeaker(IVideoPresenter presenter, AudioChannel channel)
    {
        var speaker = GetSpeaker(channel);
        if (speaker == null)
        {
            Log.Info($"Speaker {channel.ToString()} is null");
        }
        var hSnd = presenter.PlayAudio(speaker);
        ActiveSoundHandles.Add(hSnd);
    }

    public void PlayAudioOnSpeaker(IVideoPresenter presenter, int channel)
        => PlayAudioOnSpeaker(presenter, (AudioChannel)channel);

    public void StopAllSpeakerAudio()
    {
        foreach (var hSnd in ActiveSoundHandles)
        {
            hSnd.Stop(true);
        }
        ActiveSoundHandles.Clear();
    }

    public override void OnTouchStart(Entity toucher)
    {
        base.OnTouchStart(toucher);

        if (toucher is Player ply)
        {
            ply.EnterZone(this);
        }
    }

    public override void OnTouchEnd(Entity toucher)
    {
        base.OnTouchEnd(toucher);

        if (toucher is Player ply)
        {
            ply.ExitZone(this);
        }
    }
}
