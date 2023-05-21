using Sandbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class FakeBounceLight : EntityComponent, ISingletonComponent
{
    [ConVar.Client("projector.bounce.enable")]
    public static bool EnableBounce { get; set; } = true;
    [ConVar.Client("projector.bounce.debug")]
    public static int BounceDebug { get; set; } = 0;
    // You won't actually see this update in-game until issue #3288 is resolved.
    // For more details: https://github.com/sboxgame/issues/issues/3288
    [ConVar.Client("projector.bounce.cookiesize")]
    public static int BounceLightCookieSize { get; set; } = 32;
    private int _previousLightCookieSize = 32;
    [ConVar.Client("projector.bounce.brightnessfactor")]
    public static float BounceLightBrightnessFactor { get; set; } = 1f;
    [ConVar.Client("projector.bounce.fadedistancemin")]
    public static float FadeDistanceMin { get; set; } = 1500f;
    [ConVar.Client("projector.bounce.fadedistancemax")]
    public static float FadeDistanceMax { get; set; } = 2000f;
    
    public Texture SourceTexture { get; set; }
    public SpotLightEntity BounceSpotlight { get; set; }

    protected Texture DownscaledTexture { get; set; }
    protected Texture MultiplicandTexture { get; set; }
    protected Texture ProductTexture { get; set; }
    protected Texture BounceLightCookie { get; set; }

    private ComputeShader DownscaleShader { get; set; }
    private ComputeShader MultiplyShader { get; set; }
    private ComputeShader BlurShader { get; set; }
    private float BaseBounceLightBrightness { get; set; } = 10f;
    private float ScreenDistanceFromProjector { get; set; }


    public FakeBounceLight()
    {
        // Don't unnecessarily recreate textures if light cookie size isn't default.
        if (BounceLightCookieSize != _previousLightCookieSize)
        {
            _previousLightCookieSize = BounceLightCookieSize;
        }
        CreateAllShaders();
        CreateAllTextures();
    }

    protected override void OnActivate()
    {
        base.OnActivate();

        BounceSpotlight ??= CreateSpotlight();
    }

    private SpotLightEntity CreateSpotlight()
    {
        var spotlight = new SpotLightEntity()
        {
            Brightness = CalculateBounceBrightness(),
            Transform = Entity.Transform,
            Range = 1000,
            InnerConeAngle = 50f,
            OuterConeAngle = 80f,
            DynamicShadows = true,
            FogStrength = 0.25f,
            LightCookie = BounceLightCookie
        };
        spotlight.SetParent(Entity);
        return spotlight;
    }

    /// <summary>
    /// Instantiates all of the shaders required for bounce light calculation.
    /// </summary>
    private void CreateAllShaders()
    {
        DownscaleShader = new ComputeShader("downscale_cs");
        MultiplyShader = new ComputeShader("multiplytexture_cs");
        BlurShader = new ComputeShader("gaussianblur_cs");
    }

    /// <summary>
    /// Instantiates all of the textures objects required for bounce light calculation.
    /// </summary>
    private void CreateAllTextures()
    {
        DownscaledTexture   = CreateTexture();
        MultiplicandTexture = CreateTexture();
        ProductTexture      = CreateTexture();
        BounceLightCookie   = CreateTexture();
        // Downscale the mask texture to the size of the bounce light cookie.
        var largeMaskTex = Texture.Load(FileSystem.Mounted, "materials/lightcookies/box_soft.vtex");
        DownscaleTexture(largeMaskTex, MultiplicandTexture);
    }

    private Texture CreateTexture()
    {
        return Texture.Create(BounceLightCookieSize, BounceLightCookieSize)
            .WithUAVBinding()
            .WithFormat(ImageFormat.RGBA8888)
            .WithDynamicUsage()
            .Finish();
    }

    [GameEvent.Tick.Client]
    public void OnClientTick() 
    {
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

    public float CalculateBounceBrightness()
    {
        var attenuated = Math.Clamp(100f / ScreenDistanceFromProjector, 0, 1);
        return attenuated * BaseBounceLightBrightness * BounceLightBrightnessFactor;
    }

    [GameEvent.Client.Frame]
    public void OnClientFrame()
    {
        if (!EnableBounce)
        {
            return;
        }

        HandleCookieSizeUpdate();

        if (!EnsureSourceIsNotNull())
        {
            return;
        }

        UpdateBounceLightCookie();
        if (BounceDebug > 0)
        {
            ProjectorDebugOverlay();
            if (BounceDebug > 1)
            {
                TextureDebugOverlay();
            }
        }
    }

    public void HandleCookieSizeUpdate()
    {
        if (_previousLightCookieSize != BounceLightCookieSize)
        {
            CreateAllTextures();
            BounceSpotlight.LightCookie = BounceLightCookie;
        }
        _previousLightCookieSize = BounceLightCookieSize;
    }

    /// <summary>
    /// If <c>SourceTexture</c> is null, tries to set it to the <c>LightCookie</c> of
    /// <c>Entity</c>, if <c>Entity</c> is a light that supports light cookies. 
    /// Returns true if source texture is not null.
    /// </summary>
    /// <returns></returns>
    private bool EnsureSourceIsNotNull()
    {
        if (SourceTexture != null)
        {
            return true;
        }
        SourceTexture = Entity switch
        {
            OrthoLightEntity ortho => ortho.LightCookie,
            SpotLightEntity spot => spot.LightCookie,
            _ => null
        };
        return SourceTexture != null;
    }

    private void UpdateBounceLightCookie()
    {
        /* 
         *  Here we daisy-chain three compute shaders to downscale, multiply, and blur the
         *  main projector texture in order to create a fake bounce light effect.
         *  I am CERTAIN that this is not the most efficient way to do this, but it works
         *  for now, and someone with more HLSL knowledge could probably do it all in one shader.
         *  
         *  Our saving grace is that in a real theater, only one of these effects are likely to be
         *  visible at a time. God help us if someone decides to put a bunch of projectors in a small room.
         */
        DownscaleTexture(SourceTexture, DownscaledTexture);
        MultiplyTexture(DownscaledTexture, MultiplicandTexture, ProductTexture);
        BlurTexture(ProductTexture, BounceLightCookie);
    }

    private void DownscaleTexture(Texture fromTex, Texture toTex)
    {
        DownscaleShader.Attributes.Set("InputTexture", fromTex);
        DownscaleShader.Attributes.Set("OutputTexture", toTex);
        DownscaleShader.Dispatch(toTex.Width, toTex.Height, 1);
    }

    private void MultiplyTexture(Texture fromTex, Texture multTex, Texture toTex)
    {
        MultiplyShader.Attributes.Set("InputTexture", fromTex);
        MultiplyShader.Attributes.Set("MultiplicandTexture", multTex);
        MultiplyShader.Attributes.Set("OutputTexture", toTex);
        MultiplyShader.Dispatch(toTex.Width, toTex.Height, 1);
    }

    private void BlurTexture(Texture fromTex, Texture toTex)
    {
        BlurShader.Attributes.Set("InputTexture", fromTex);
        BlurShader.Attributes.Set("OutputTexture", toTex);
        BlurShader.Dispatch(toTex.Width, toTex.Height, 1);
    }

    private void ProjectorDebugOverlay()
    {
        DebugOverlay.Sphere(Entity.Position, 5f, Color.Blue);
        DebugOverlay.Line(Entity.Position, BounceSpotlight.Position, Color.Yellow);
        DebugOverlay.Circle(BounceSpotlight.Position, BounceSpotlight.Rotation, BounceSpotlight.OuterConeAngle, Color.Red);
    }

    private void TextureDebugOverlay()
    {
        DebugOverlay.Texture(SourceTexture, new Vector2(0, 0));
        int texPosY = 0;
        DebugOverlay.Texture(DownscaledTexture, new Vector2(SourceTexture.Width, texPosY));
        texPosY += DownscaledTexture.Height;
        DebugOverlay.Texture(ProductTexture, new Vector2(SourceTexture.Width, texPosY));
        texPosY += ProductTexture.Height;
        DebugOverlay.Texture(BounceLightCookie, new Vector2(SourceTexture.Width, texPosY));
    }
}
