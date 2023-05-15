using Sandbox;

namespace Cinema;

public partial class ProjectorEntity
{
    protected WebMediaSource MediaSource { get; set; }

    public string CurrentUrl
    {
        get => MediaSource.CurrentUrl;
        set => MediaSource.CurrentUrl = value;
    }

    private SceneWorld ProjectorSceneWorld { get; set; }
    private SceneCamera ProjectorSceneCamera { get; set; }
    private Texture ProjectionTexture { get; set; }
    private OrthoLightEntity ProjectionLight { get; set; }

    private void InitProjection()
    {
        //Initialize Scene World
        ProjectorSceneWorld = new SceneWorld();

        //Initialize Scene Camera
        ProjectorSceneCamera = new SceneCamera();
        ProjectorSceneCamera.World = ProjectorSceneWorld;
        ProjectorSceneCamera.Position = new Vector3();
        ProjectorSceneCamera.ZNear = 1.0f;
        ProjectorSceneCamera.ZFar = 15000.0f;

        //Initialize Texture
        ProjectionTexture = Texture.CreateRenderTarget("projection-img", ImageFormat.RGBA8888, ProjectionResolution);

        //Initialize Media Panel
        MediaSource = new WebMediaSource(this);
        MediaSource.Position = ProjectorSceneCamera.Position + (Vector3.Forward * 36.0f);
        MediaSource.Rotation = Rotation.FromYaw(180);

        ProjectionLight = new OrthoLightEntity
        {
            Parent = this,
            Position = Position,
            Rotation = Rotation,
            LightCookie = ProjectionTexture,
            Brightness = 1.0f,
            Range = 1024.0f,
            OrthoLightWidth = ProjectionSize.x,
            OrthoLightHeight = ProjectionSize.y,
            DynamicShadows = true,
        };

        ProjectionLight.UseFog();
    }

    [GameEvent.PreRender]
    protected void RenderScene()
    {
        Graphics.RenderToTexture(ProjectorSceneCamera, ProjectionTexture);
    }
}
