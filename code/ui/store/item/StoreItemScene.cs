using Sandbox;
using Sandbox.UI;

namespace Cinema.UI;

public partial class StoreItemScene : Panel
{
    private Texture DebugTexture { get; set; }

    private ScenePanel ItemScenePanel { get; set; }
    private SceneWorld ItemSceneWorld { get; set; }

    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        InitScene();
    }
    private void InitScene()
    {
        Log.Info("Initializing Concession Item Scene...");

        Style.Width = Length.Percent(100);
        Style.Height = Length.Percent(100);

        //Initialize World
        ItemSceneWorld?.Delete();
        ItemSceneWorld = new SceneWorld();

        //Initialize Camera
        var eyePosition = new Vector3(0, 0, 64);
        var offset = new Vector3(72, -20, -10);

        ItemScenePanel.Camera.Position = eyePosition + offset;
        ItemScenePanel.Camera.Rotation = Rotation.LookAt((eyePosition - ItemScenePanel.Camera.Position).Normal, Vector3.Up);

        ItemScenePanel.Camera.FieldOfView = 20f;
        ItemScenePanel.Camera.ZNear = 0.1f;
        ItemScenePanel.Camera.ZFar = 1024.0f;

        //Initialize Lighting
        _ = new SceneLight(ItemSceneWorld, Vector3.Up * 128 + Vector3.Forward * 60, 100, new Color(1f, 0.85f, 0.71f) * 16.0f);

        //Initialize Models
        _ = new SceneModel(ItemSceneWorld, "models/citizen/citizen.vmdl", Transform.Zero);
    }
}
