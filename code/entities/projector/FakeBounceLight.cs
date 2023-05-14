using Sandbox;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class FakeBounceLight : Entity
{
    public OrthoLightEntity Light { get; set; }
    public Texture SourceTexture { get; set; }
    public Texture IntermediateTexture { get; set; }
    public int SampleSize { get; set; } = 8;
    public SpotLightEntity Spotlight { get; set; }
    private ComputeShader DownscaleShader { get; set; }
    private ComputeShader BlurShader { get; set; }

    public void Init()
    {
        Spotlight = new SpotLightEntity()
        {
            Transform = Transform
        };
        Spotlight.SetParent(this);
        DownscaleShader = new ComputeShader("projectordownscale_cs");
        BlurShader = new ComputeShader("projectorblur_cs");
        Spotlight.LightCookie = CreateTexture();
        IntermediateTexture = CreateTexture();
    }

    [GameEvent.Tick.Client]
    public void OnClientTick() 
    {
        var traceStart = Light.Position;
        var traceEnd = Light.Position + Light.Rotation.Forward * 5000f;
        var tr = Trace.Ray(traceStart, traceEnd)
            .Run();
        if (!tr.Hit)
        {
            return;
        }
        Position = tr.HitPosition;
        Rotation = Rotation.LookAt(tr.Normal);
    }

    private Texture CreateTexture()
    {
        return Texture.Create(64, 64)
            .WithUAVBinding()
            .WithFormat(ImageFormat.RGBA8888)
            .WithDynamicUsage()
            .Finish();
    }

    [GameEvent.Client.Frame]
    public void UpdateLightCookie()
    {
        DownscaleShader.Attributes.Set("InputTexture", SourceTexture);
        DownscaleShader.Attributes.Set("OutputTexture", IntermediateTexture);
        DownscaleShader.Dispatch(IntermediateTexture.Width, IntermediateTexture.Height, 1);
        BlurShader.Attributes.Set("InputTexture", IntermediateTexture);
        BlurShader.Attributes.Set("OutputTexture", Spotlight.LightCookie);
        BlurShader.Dispatch(Spotlight.LightCookie.Width, Spotlight.LightCookie.Height, 1);
        //DebugOverlay.Texture(SourceTexture, new Vector2(0, 0));
        //DebugOverlay.Texture(Spotlight.LightCookie, new Vector2(0, SourceTexture.Height));
    }
}
