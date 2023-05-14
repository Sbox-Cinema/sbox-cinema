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
    public int SampleSize { get; set; } = 8;
    public SpotLightEntity Spotlight { get; set; }
    private ComputeShader Shader { get; set; }

    public void Init()
    {
        Spotlight = new SpotLightEntity()
        {
            Transform = Transform
        };
        Spotlight.SetParent(this);
        Shader = new ComputeShader("projectorbounce_cs");
        Spotlight.LightCookie = CreateLightCookie();
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

    private Texture CreateLightCookie()
    {
        return Texture.Create(32, 32)
            .WithUAVBinding()
            .WithFormat(ImageFormat.RGBA8888)
            .WithDynamicUsage()
            .Finish();
    }

    [GameEvent.Client.Frame]
    public void UpdateLightCookie()
    {
        var outTex = Spotlight.LightCookie;
        Shader.Attributes.Set("InputTexture", SourceTexture);
        Shader.Attributes.Set("OutputTexture", outTex);
        Shader.Attributes.Set("GameTime", Time.Now);
        Shader.Dispatch(outTex.Width, outTex.Height, 1);
        //DebugOverlay.Texture(SourceTexture, new Vector2(0, 0));
        //DebugOverlay.Texture(outTex, new Vector2(0, SourceTexture.Height));
    }
}
