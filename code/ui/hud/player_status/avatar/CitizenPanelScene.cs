using Sandbox;
using Sandbox.UI;

namespace Cinema.UI;
public partial class CitizenPanelScene : ScenePanel
{
    public Vector3 OrbitPosition { get; set; }
    public Vector3 CameraOffset { get; set; }
    private SceneModel CitizenModel { get; set; }
    private ClothingContainer CitizenClothing { get; set; } = new ClothingContainer();

    public CitizenPanelScene()
    {
        AddClass("avatar");
        InitScene();
    }

    private void InitScene()
    {
        //Initialize Scene Settings
        RenderOnce = true;

        //Initialize World
        World?.Delete();
        World = new();

        //Initialize Lighting
        _ = new SceneLight(World, Vector3.Up * 128 + Vector3.Forward * 60, 100, new Color(1f, 0.85f, 0.71f) * 16.0f);

        //Initialize Citizen
        CitizenModel = new SceneModel(World, "models/citizen/citizen.vmdl", Transform.Zero);
        CitizenModel.SetAnimGraph("models/citizen/citizen.vanmgrph");
        CitizenModel.Update(RealTime.Delta);

        //Initialize Citizen's Clothing
        SetupClothing();
    }

    [Event.Hotload]
    private void Update()
    {
        Cleanup();
        InitScene();
    }

    private void Cleanup()
    {
        CitizenModel?.Delete();
        CitizenModel = null;
    }

    private void SetupClothing()
    {
        var player = Game.LocalClient.Pawn as Player;

        CitizenClothing.LoadFromClient(Game.LocalClient);
        CitizenClothing.Deserialize(player.ClothingAsString);

        UpdateClothing();
    }

    private void UpdateClothing()
    {
        var models = CitizenClothing.DressSceneObject(CitizenModel);

        var delta = RealTime.Delta;
        
        models.ForEach(x => x.Update(delta));
    }

    private void UpdateSceneCamera()
    {
        var eyePosition = OrbitPosition;
        var offset = CameraOffset;

        Camera.Position = eyePosition + offset;
        Camera.Rotation = Rotation.LookAt((eyePosition - Camera.Position).Normal, Vector3.Up);
        Camera.FieldOfView = 20f;
        Camera.ZNear = 0.1f;
        Camera.ZFar = 512.0f;
    }

    public override void Tick()
    {
        base.Tick();

        UpdateSceneCamera();
    }
}
