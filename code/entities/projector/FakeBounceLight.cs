using Sandbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class FakeBounceLight : Entity
{
    [ConVar.Client("cinema.projector.bouncetest")]
    public static bool EnableBounceTest { get; set; } = false;
    public OrthoLightEntity ProjectorLight { get; set; }
    public Texture SourceTexture { get; set; }
    public Texture DownscaledTexture { get; set; }
    public Texture ProductTexture { get; set; }
    public Texture BounceLightCookie { get; set; }
    public SpotLightEntity Spotlight { get; set; }
    private ComputeShader DownscaleShader { get; set; }
    private ComputeShader MultiplyShader { get; set; }
    private ComputeShader BlurShader { get; set; }
    public Texture MultiplicandTexture { get; set; }

    public FakeBounceLight()
    {
        DownscaleShader = new ComputeShader("downscale_cs");
        MultiplyShader = new ComputeShader("multiplytexture_cs");
        BlurShader = new ComputeShader("gaussianblur_cs");
        BounceLightCookie = CreateTexture();
        DownscaledTexture = CreateTexture();
        MultiplicandTexture = CreateTexture();
        ProductTexture = CreateTexture();
        var maskLargeTex = Texture.Load(FileSystem.Mounted, "materials/effects/dirt1.vtex");
        DownscaleTexture(maskLargeTex, MultiplicandTexture);
        Spotlight = CreateSpotlight();
    }

    private SpotLightEntity CreateSpotlight()
    {
        var spotlight = new SpotLightEntity()
        {
            Brightness = 2,
            Transform = Transform,
            Range = 1000,
            InnerConeAngle = 50f,
            OuterConeAngle = 80f,
            DynamicShadows = true,
            FogStrength = 0.25f,
            LightCookie = BounceLightCookie
        };
        spotlight.SetParent(this);
        return spotlight;
    }

    private Texture CreateTexture()
    {
        return Texture.Create(64, 64)
            .WithUAVBinding()
            .WithFormat(ImageFormat.RGBA8888)
            .WithDynamicUsage()
            .Finish();
    }

    [GameEvent.Tick.Client]
    public void OnClientTick() 
    {
        var traceStart = ProjectorLight.Position;
        var traceEnd = ProjectorLight.Position + ProjectorLight.Rotation.Forward * 5000f;
        var tr = Trace.Ray(traceStart, traceEnd)
            .WorldOnly()
            .Run();
        if (!tr.Hit)
        {
            return;
        }
        Position = tr.HitPosition;
        Rotation = Rotation.LookAt(tr.Normal);
    }

    [GameEvent.Client.Frame]
    public void OnClientFrame()
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
        if (EnableBounceTest)
        {
            TextureDebugOverlay();
        }
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
