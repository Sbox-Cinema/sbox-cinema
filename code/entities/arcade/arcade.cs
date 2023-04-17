using Sandbox;
using Sandbox.UI;
using System.Linq;
using System.Reflection.PortableExecutable;

namespace Cinema;

public partial class ArcadeMachine : ModelEntity
{
    [ConCmd.Server("arcade")]
    public static void MakeConsole()
    {
        var aimRay = Trace.Ray((ConsoleSystem.Caller.Pawn as Player).AimRay, 10000).WorldOnly().Run();
        new ArcadeMachine()
        {
            Position = aimRay.EndPosition,
        };
    }

    public Texture ScreenTexture { get; set; }

    public WorldPanel ScreenWorldPanel { get; set; }
    public ArcadeScreen Screen { get; set; }

    public override void Spawn()
    {
        SetModel("models/gamemodes/arcade/arcade1.vmdl_c");
        PhysicsEnabled = true;
        UsePhysicsCollision = true;
        base.Spawn();
    }

    public override void ClientSpawn()
    {
        ScreenWorldPanel = new WorldPanel();
        Screen = new ArcadeScreen(this);
        ScreenWorldPanel.AddChild(Screen);

        ScreenTexture = Texture.CreateRenderTarget(
                "arcade-screen",
                ImageFormat.RGBA8888,
                new Vector2(512, 512)
            );


    }

    [Event.PreRender]
    public void Render()
    {
        ScreenWorldPanel.WorldScale = 0.5f;
        ScreenWorldPanel.Position = Position + Vector3.Up * 65 + Rotation.Forward * 8;

        var camera = new SceneCamera();
        camera.World = Game.SceneWorld;
        camera.Position = Position + Vector3.Up * 60 + Rotation.Forward * 20;
        camera.Rotation = Rotation.From(new Angles(0, 0, 0));
        camera.FieldOfView = 120;
        camera.ZFar = 10000;

        Graphics.RenderToTexture(camera, ScreenTexture);
    }

    [Event.Client.Frame]
    void FrameTick()
    {

    }
}

public partial class ArcadeScreen : Panel
{
    public override bool HasContent => true;
    public ArcadeMachine Machine { get; set; }
    public ArcadeScreen(ArcadeMachine machine)
    {
        Machine = machine;
    }

    public override void Tick()
    {
        Style.BackgroundImage = Machine.ScreenTexture;
        Style.BackgroundSizeX = 512;
        Style.BackgroundSizeY = 512;
        Style.Width = 512;
        Style.Height = 512;

        base.Tick();
    }

    public override void DrawContent(ref RenderState state)
    {



        //var m = Material.UI.Basic;
        //var a = new RenderAttributes();
        //a.Set("BoxPosition", new Vector2(state.X, state.Y));
        //a.Set("BoxSize", new Vector2(state.Width, state.Height));
        //a.Set("Texture", Machine.ScreenTexture, -1);
        //a.Set("D_BLENDMODE", (int)BlendMode.Normal);
        //a.Set("D_BACKGROUND_IMAGE", 1);
        //a.Set("BgPos", new Vector4(0, 0, state.Width, state.Height));
        //Graphics.DrawQuad(new Rect(new Vector2(state.X, state.Y), new Vector2(state.Width, state.Height)), m, Color.White, a);



        base.DrawContent(ref state);
    }
}


