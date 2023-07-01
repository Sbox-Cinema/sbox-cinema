using Sandbox;
using Sandbox.Internal;

namespace CinemaTeam.Plugins.Video;

public class DummyVideoPresenter : IVideoPresenter
{
    [ConVar.Client("plugins.video.test.drawtex")]
    public static bool EnableShaderDebug { get; set; }
    public Texture MultiplicandTexture { get; set; }
    public Texture Texture { get; private set; }

    private ComputeShader ColorfulShader { get; set; }

    public DummyVideoPresenter()
    {
        ColorfulShader = new ComputeShader("colorfultest_cs");
        MultiplicandTexture = Texture.Load(FileSystem.Mounted, "textures/border_collies.vtex");
        Texture = Texture.Create(MultiplicandTexture.Width, MultiplicandTexture.Height)
            .WithUAVBinding()
            .WithDynamicUsage()
            .WithFormat(ImageFormat.RGBA8888)
            .Finish();
        Event.Register(this);
    }

    public SoundHandle? PlayAudio(IEntity entity)
    {
        return Audio.Play("waltz_of_the_flowers", entity);
    }

    [GameEvent.Client.Frame]
    public void OnFrame()
    {
        ColorfulShader.Attributes.Set("GameTime", Time.Now);
        ColorfulShader.Attributes.Set("MultiplicandTexture", MultiplicandTexture);
        ColorfulShader.Attributes.Set("OutputTexture", Texture);
        ColorfulShader.Dispatch(Texture.Width, Texture.Height, 1);
        if (EnableShaderDebug)
        {
            DebugOverlay.Texture(Texture, Vector2.Zero);
        }
    }
}
