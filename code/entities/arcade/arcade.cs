using Sandbox;
using Sandbox.UI;

namespace Cinema;

public partial class ArcadeMachine : ModelEntity, ICinemaUse
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

    public SceneWorld SecondarySceneWorld { get; set; }

    public ArcadePanel ArcadePanel { get; set; }

    [Net]
    public MyArcadeGame GameController { get; set; }

    public UI.ArcadeTest ArcadeTestPanel { get; set; }

    public WorldPanel DebugWorldPanel { get; set; }


    public override void Spawn()
    {
        SetModel("models/gamemodes/arcade/arcade1.vmdl_c");
        PhysicsEnabled = true;
        UsePhysicsCollision = true;

        GameController = new MyArcadeGame();

        base.Spawn();
    }

    public Material ScreenMat { get; set; }

    public override void ClientSpawn()
    {
        DebugWorldPanel = new WorldPanel();
        var debugScreen = new ArcadeScreen(this);
        DebugWorldPanel.AddChild(debugScreen);

        SecondarySceneWorld = new SceneWorld();

        ScreenMat = Material.FromShader("shaders/lcd_monitor.shader_c");

        ArcadePanel = new ArcadePanel(SecondarySceneWorld);
        ArcadeTestPanel = new UI.ArcadeTest(GameController);
        ArcadePanel.AddChild(ArcadeTestPanel);
    }

    [Event.PreRender]
    public void Render()
    {

        ArcadePanel.PanelBounds = new Rect(new Vector2(-500, -400), new Vector2(1000, 800));
        ArcadePanel.Position = new Vector3(0, 0, 0);
        ArcadePanel.WorldScale = 2f;

        ScreenTexture = Texture.CreateRenderTarget(
                $"arcade-screen-{NetworkIdent}",
                ImageFormat.RGBA8888,
                new Vector2(1000, 800)
            );

        var sCam = new SceneCamera();
        sCam.World = SecondarySceneWorld;
        sCam.Position = Vector3.Forward * 80;
        sCam.Rotation = Rotation.From(new Angles(0, 180, 0));
        sCam.ZFar = 10000;
        sCam.ZNear = 0.1f;
        Graphics.RenderToTexture(sCam, ScreenTexture);

        var t = Texture.Load(FileSystem.Mounted, "materials/pixel-1.vtex");
        ScreenMat.Set("Color", ScreenTexture);
        ScreenMat.Set("TintMask", new Color(0, 0, 0));
        ScreenMat.Set("Pixel", t);


        ScreenMat.Set("g_flFresnelExponent", 20f);
        ScreenMat.Set("g_flFresnelReflectance", 0.01f);

        ScreenMat.Set("g_flPixelBlendDistance", 4f);
        ScreenMat.Set("g_flPixelBlendOffset", 0f);
        ScreenMat.Set("g_flPixelOpacityMin", 1f);
        ScreenMat.Set("g_flPixelOpacityMax", 1f);
        ScreenMat.Set("g_vPixelSize", new Vector2(1f, 1f));
        ScreenMat.Set("g_vScreenResolution", new Vector2(1000f, 1000f));

        ScreenMat.Set("g_vAnimationScroll", new Vector2(0f, 0f));

        ScreenMat.Set("g_flBrightnessMultiplier", 1f);

        SceneObject.SetMaterialOverride(ScreenMat, "screen");

    }

    [Event.Client.Frame]
    void FrameTick()
    {
        DebugWorldPanel.PanelBounds = new Rect(new Vector2(-500, -400), new Vector2(1000, 800));
        DebugWorldPanel.WorldScale = 0.5f;
        DebugWorldPanel.Position = Position + Rotation.Up * 65 + Rotation.Forward * 2;
        DebugWorldPanel.Rotation = Rotation.RotateAroundAxis(Vector3.Right, 5f);
        ArcadeTestPanel.GameController = GameController;
    }


    public bool IsUsable(Entity user)
    {
        return true;
    }

    public bool OnUse(Entity user)
    {
        Log.Info("used");
        GameController.GameStarted = true;
        return false;
    }

    public void OnStopUse(Entity user)
    {

    }



}



public partial class MyArcadeGame : BaseNetworkable
{
    [Net]
    public bool GameStarted { get; set; } = false;
}

public partial class ArcadePanel : WorldPanel
{
    public ArcadePanel(SceneWorld world = null) : base(world)
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
        Style.BackgroundSizeX = Length.Cover;
        Style.BackgroundSizeY = Length.Cover;
        Style.Width = Length.Percent(100);
        Style.Height = Length.Percent(100);

        base.Tick();
    }

    public override void DrawContent(ref RenderState state)
    {
        base.DrawContent(ref state);
    }
}


