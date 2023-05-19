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
    public static bool EnableBounceDebug { get; set; } = false;
    // You won't actually see this update in-game until issue #3288 is resolved.
    // For more details: https://github.com/sboxgame/issues/issues/3288
    [ConVar.Client("projector.bounce.cookiesize")]
    public static int BounceLightCookieSize { get; set; } = 64;
    private int _previousLightCookieSize = 64;
    
    public Texture SourceTexture { get; set; }
    public SpotLightEntity Spotlight { get; set; }

    protected Texture DownscaledTexture { get; set; }
    protected Texture MultiplicandTexture { get; set; }
    protected Texture ProductTexture { get; set; }
    protected Texture BounceLightCookie { get; set; }

    private ComputeShader DownscaleShader { get; set; }
    private ComputeShader MultiplyShader { get; set; }
    private ComputeShader BlurShader { get; set; }


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

        if (Spotlight == null)
        {
            Spotlight = CreateSpotlight();
        }
    }

    private SpotLightEntity CreateSpotlight()
    {
        var spotlight = new SpotLightEntity()
        {
            Brightness = 2,
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
        // Load the mask texture and downscale it to the size of the bounce light cookie.
        var largeMaskTex = Texture.Load(FileSystem.Mounted, "materials/effects/dirt1.vtex");
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
        if (EnableBounce)
        {
            Spotlight.Enabled = true;
            Spotlight.Brightness = 2;
        }
        else
        {
            Spotlight.Enabled = false;
            Spotlight.Brightness = 0;
            return;
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
        // Make sure the bounce spotlight is at the surface and pointing away from it.
        Spotlight.Position = tr.HitPosition;
        Spotlight.Rotation = Rotation.LookAt(tr.Normal);
    }

    [GameEvent.Client.Frame]
    public void OnClientFrame()
    {
        if (!EnableBounce)
        {
            return;
        }

        // Check to see if the light cookie size has changed.
        if (_previousLightCookieSize != BounceLightCookieSize)
        {
            CreateAllTextures();
            Spotlight.LightCookie = BounceLightCookie;
        }
        _previousLightCookieSize = BounceLightCookieSize;

        if (!EnsureSourceIsNotNull())
        {
            return;
        }

        UpdateBounceLightCookie();
        if (EnableBounceDebug)
        {
            TextureDebugOverlay();
        }
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
