using System;
using Sandbox;
using System.Threading.Tasks;

namespace Cinema;

internal enum FadeState
{
    None,
    Fading
};

public partial class AdjustableSound
{
    private FadeState FadeState { get; set; } = FadeState.None;

    public string SoundName { get; protected set; }
    public Sound Sound { get; protected set; }
    public float Volume { get; protected set; }
    public float ActualVolume { get; protected set; }

    public bool Loops { get; set; } = true;

    public TimeSince TimeSinceStarted { get; protected set; }

    public string ConfigKey { get; protected set; }

    // The rate at which the fade will change per second
    private float FadeRate = 0;

    // The target fade ratio, moving to this at FadeRate per second
    private float FadeTarget = 0.0f;

    // Volume multiplier applied by fade
    private float FadeRatio = 1.0f;

    public AdjustableSound(string soundName, float volume = 1.0f, float fadeInTime = 0.0f)
    {
        SoundName = soundName;
        Sound = Sound.FromScreen(soundName);
        Volume = volume;

        TimeSinceStarted = 0;

        if (fadeInTime != 0)
        {
            FadeRatio = 0.0f;
            FadeToLevel(fadeInTime, 1.0f);
        }

        ActualVolume = CalculateCurrentVolume();
        Sound.SetVolume(ActualVolume);

        Event.Register(this);
    }

    ~AdjustableSound()
    {
        Event.Unregister(this);
    }

    public void TieToConfig(string configKey)
    {
        ConfigKey = configKey;
    }

    public void FadeToLevel(float fadeTime = 10.0f, float fadeLevel = 0.0f)
    {
        FadeState = FadeState.Fading;
        FadeTarget = fadeLevel.Clamp(0, 1);
        FadeRate = Math.Abs(FadeTarget - FadeRatio) / fadeTime;
    }

    [Event.Tick.Client]
    private void Update()
    {
        if (Loops)
        {
            UpdateLoop();
        }

        UpdateFade();
        var currentVolume = CalculateCurrentVolume();
        if (currentVolume != ActualVolume)
        {
            ActualVolume = currentVolume;
            Sound.SetVolume(ActualVolume);
        }
    }

    private float CalculateCurrentVolume()
    {
        var newVolume = Volume;
        if (ConfigKey != null && Settings.Current.Values.ContainsKey(ConfigKey))
        {
            var configLevel = (float)Settings.Current.Values[ConfigKey];
            newVolume *= configLevel;
        }
        newVolume *= FadeRatio;
        newVolume = newVolume.Clamp(0.0f, 1.0f);

        return newVolume;
    }

    private void UpdateFade()
    {
        if (FadeState != FadeState.Fading)
            return;

        FadeRatio = FadeRatio.Approach(FadeTarget, FadeRate * Time.Delta);

        if (FadeRatio == FadeTarget)
            FadeState = FadeState.None;
    }

    private void UpdateLoop()
    {
        if (Sound.IsPlaying)
            return;

        Sound = Sound.FromScreen(SoundName);
        Sound.SetVolume(ActualVolume);
    }
}
