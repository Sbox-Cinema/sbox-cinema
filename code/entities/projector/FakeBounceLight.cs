using Sandbox;
using System;

namespace Cinema;

public partial class FakeBounceLight : EntityComponent, ISingletonComponent
{
    /// <summary>
    /// If enabled, a fake bounce light effect will be applied to all projectors.
    /// </summary>
    [ConVar.Client("projector.bounce.enable")]
    public static bool EnableBounce { get; set; } = true;
    /// <summary>
    /// Depending on the setting, displays debug information about the fake bounce light.<br/>
    /// 0 - No debug info<br/>
    /// 1 - Displays the positions of the projector and the bounce light plus path between them.<br/>
    /// 2 - Displays all previously mentioned info and also the contents of the various textures 
    /// used by the shaders.
    /// </summary>
    [ConVar.Client("projector.bounce.debug")]
    public static int BounceDebug { get; set; } = 0;
    // You won't actually see this update in-game until issue #3288 is resolved.
    // For more details: https://github.com/sboxgame/issues/issues/3288
    /// <summary>
    /// Determines the size of the texture used as the light cookie of <c>BounceSpotlight</c>.
    /// A higher resolution may be more temporally stable, preventing some light flickering, but 
    /// also being less realistic.
    /// </summary>
    [ConVar.Client("projector.bounce.cookiesize")]
    public static int BounceLightCookieSize { get; set; } = 32;
    // We use this instance field to check if the cookie size has changed since the last frame.
    private int _PreviousLightCookieSize = 32;
    /// <summary>
    /// A factor that shall be applied at the end of brightness calculations to make the bounce
    /// light brighter or dimmer.
    /// </summary>
    [ConVar.Client("projector.bounce.brightnessfactor")]
    public static float BounceLightBrightnessFactor { get; set; } = 1f;
    /// <summary>
    /// Used to set the <c>FadeDistanceMin</c> of <c>BounceSpotlight</c>.
    /// </summary>
    [ConVar.Client("projector.bounce.fadedistancemin")]
    public static float FadeDistanceMin { get; set; } = 1500f;
    /// <summary>
    /// Used to set the <c>FadeDistanceMax</c> of <c>BounceSpotlight</c>.
    /// </summary>
    [ConVar.Client("projector.bounce.fadedistancemax")]
    public static float FadeDistanceMax { get; set; } = 2000f;
    public static int Iteration { get; set; } = 0;
    private int _Iteration = 0;

    /// <summary>
    /// The texture that shall be copied from when rendering the bounce light. 
    /// Generally, this should be the <c>LightCookie</c> of a light that represents
    /// the projector.
    /// </summary>
    public Texture SourceTexture { get; set; }
    /// <summary>
    /// The light that projects the <c>BounceLightCookie</c> away from the screen to create the
    /// fake bounce light effect.
    /// </summary>
    public SpotLightEntity BounceSpotlight 
    {
        get => _BounceSpotlight; 
        private set
        {
            _BounceSpotlight?.Delete();
            _BounceSpotlight = value;
        }
    }
    private SpotLightEntity _BounceSpotlight;
    /// <summary>
    /// The distance between <c>Entity</c> and the screen on to which it projects.
    /// </summary>
    public float ScreenDistanceFromProjector { get; private set; }

    private float BaseBounceLightBrightness { get; init; } = 10f;

    public FakeBounceLight()
    {
        InitRendering();
    }

    protected override void OnActivate()
    {
        base.OnActivate();

        BounceSpotlight ??= CreateSpotlight();
    }

    [ConCmd.Client("projector.bounce.reload")]
    public static void ForceReload()
    {
        Iteration++;   
    }

    public void Reload()
    {
        InitRendering();
        BounceSpotlight = CreateSpotlight();
    }

    private SpotLightEntity CreateSpotlight()
    {
        var spotlight = new SpotLightEntity()
        {
            Brightness = CalculateBounceBrightness(),
            Transform = Entity.Transform,
            Range = 1000,
            FadeDistanceMin = FadeDistanceMin,
            FadeDistanceMax = FadeDistanceMax,
            InnerConeAngle = 50f,
            OuterConeAngle = 80f,
            DynamicShadows = true,
            FogStrength = 0.25f,
            LightCookie = BounceLightCookie
        };
        spotlight.Transmit = TransmitType.Always;
        spotlight.SetParent(Entity);
        return spotlight;
    }

    [GameEvent.Tick.Client]
    public void OnClientTick()
    {
        if (Iteration != _Iteration)
        {
            _Iteration = Iteration;
            Reload();
        }

        // Trace forward from the projector light to find the surface it projects on to.
        var traceStart = Entity.Position;
        var traceEnd = Entity.Position + Entity.Rotation.Forward * 5000f;
        var tr = Trace.Ray(traceStart, traceEnd)
            .WorldOnly()
            .Run();
        if (!tr.Hit)
        {
            return;
        }

        ScreenDistanceFromProjector = tr.Distance;

        // Make sure the bounce spotlight is at the surface and pointing away from it.
        BounceSpotlight.Position = tr.HitPosition;
        BounceSpotlight.Rotation = Rotation.LookAt(tr.Normal);

        var distFromCameraToBounceLight = Camera.Position.Distance(BounceSpotlight.Position);
        if (EnableBounce && distFromCameraToBounceLight < FadeDistanceMax)
        {
            BounceSpotlight.Enabled = true;
            BounceSpotlight.Brightness = CalculateBounceBrightness();
            BounceSpotlight.FadeDistanceMin = FadeDistanceMin;
            BounceSpotlight.FadeDistanceMax = FadeDistanceMax;
        }
        else
        {
            BounceSpotlight.Enabled = false;
            BounceSpotlight.Brightness = 0;
            return;
        }
    }

    /// <summary>
    /// Returns the appropriate brightness for <c>BounceSpotlight</c> based on 
    /// <c>ScreenDistanceFromProjector</c> and <c>BounceLightBrightnessFactor</c>.
    /// </summary>
    /// <returns></returns>
    public float CalculateBounceBrightness()
    {
        var attenuation = Math.Clamp(100f / ScreenDistanceFromProjector, 0, 1);
        return attenuation * BaseBounceLightBrightness * BounceLightBrightnessFactor;
    }
}
