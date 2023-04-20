namespace Cinema;

public partial class Projection : Entity
{
	public ProjectorEntity ProjectorEntity { get; set; }
	public WebMediaSource MediaSource { get; set; }
	private SceneWorld ProjectorSceneWorld { get; set; }
	private SceneCamera ProjectorSceneCamera { get; set; }
	private Texture ProjectionTexture { get; set; }
	private OrthoLightEntity ProjectionLight { get; set; }

	public Projection(ProjectorEntity ent, WebMediaSource mediaSrc)
	{
		ProjectorEntity = ent;
		MediaSource = mediaSrc;

		InitProjectionScene();
		InitProjectionImage();
	}
	private void InitProjectionScene()
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
		ProjectionTexture = Texture.CreateRenderTarget( "projection-img", ImageFormat.RGBA8888, ProjectorEntity.ProjectionResolution );

		//Initialize Media Panel
		MediaSource = new WebMediaSource( ProjectorEntity, ProjectorSceneWorld );
		MediaSource.Position = ProjectorSceneCamera.Position + (Vector3.Forward * 36.0f);
		MediaSource.Rotation = Rotation.FromYaw( 180 );
	}
	private void InitProjectionImage()
	{
		ProjectionLight = new OrthoLightEntity
		{
			Parent = ProjectorEntity,
			Position = ProjectorEntity.Position,
			Rotation = Rotation.From( ProjectorEntity.Rotation.Angles() ),
			LightCookie = ProjectionTexture,
			Brightness = 1.0f,
			Range = 1024.0f,
			OrthoLightWidth = ProjectorEntity.ProjectionSize.x,
			OrthoLightHeight = ProjectorEntity.ProjectionSize.y,
			DynamicShadows = true,
		};

		ProjectionLight.UseFog();
	}

	[Event.PreRender]
	private void RenderScene()
	{
		Graphics.RenderToTexture( ProjectorSceneCamera, ProjectionTexture );
	}
}
