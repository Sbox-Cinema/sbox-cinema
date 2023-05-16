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
    public Texture IntermediateTexture { get; set; }
    public Texture LightCookie { get; set; }
    public SpotLightEntity Spotlight { get; set; }
    private ComputeShader DownscaleShader { get; set; }
    private ComputeShader BlurShader { get; set; }
    public Texture MaskTexture { get; set; }

    public FakeBounceLight()
    {
        DownscaleShader = new ComputeShader("projectordownscale_cs");
        BlurShader = new ComputeShader("projectorblur_cs");
        LightCookie = Texture.CreateRenderTarget()
            .WithSize(64)
            .WithFormat(ImageFormat.RGBA8888)
            .WithDynamicUsage()
            .WithUAVBinding()
            .Create();
        IntermediateTexture = CreateTexture();
        MaskTexture = CreateTexture();
        var maskLargeTex = Texture.Load(FileSystem.Mounted, "materials/effects/dirt1.vtex");
        DownscaleTexture(maskLargeTex, MaskTexture);
        Spotlight = CreateSpotlight();
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
        DownscaleTexture(SourceTexture, IntermediateTexture);
        BlurTexture(IntermediateTexture, LightCookie, MaskTexture);
        if (EnableBounceTest)
        {
            DebugOverlay.Texture(SourceTexture, new Vector2(0, 0));
            DebugOverlay.Texture(IntermediateTexture, new Vector2(SourceTexture.Width, 0));
            DebugOverlay.Texture(LightCookie, new Vector2(SourceTexture.Width, IntermediateTexture.Height));
        }
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
            LightCookie = LightCookie
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

    private void DownscaleTexture(Texture fromTex, Texture toTex)
    {
        DownscaleShader.Attributes.Set("InputTexture", fromTex);
        DownscaleShader.Attributes.Set("OutputTexture", toTex);
        DownscaleShader.Dispatch(toTex.Width, toTex.Height, 1);
    }

    private void BlurTexture(Texture fromTex, Texture toTex, Texture maskTex)
    {
        BlurShader.Attributes.Set("InputTexture", fromTex);
        BlurShader.Attributes.Set("MaskTexture", maskTex);
        BlurShader.Attributes.Set("OutputTexture", toTex);
        BlurShader.Dispatch(toTex.Width, toTex.Height, 1);
    }
}
