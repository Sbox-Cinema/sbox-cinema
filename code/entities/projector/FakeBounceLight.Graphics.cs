using Sandbox;

namespace Cinema;

public partial class FakeBounceLight
{
    /// <summary>
    /// A downscaled version of the <c>LightCookie</c> of <c>Entity</c>.
    /// </summary>
    private Texture DownscaledTexture { get; set; }
    /// <summary>
    /// A mask texture that <c>DownscaledTexture</c> shall be multiplied by.
    /// </summary>
    private Texture MultiplicandTexture { get; set; }
    /// <summary>
    /// The final product of <c>DownscaledTexture</c> multiplied by <c>MultiplicandTexture</c>.
    /// </summary>
    private Texture ProductTexture { get; set; }
    /// <summary>
    /// This should hold a reference to the light cookie used by <c>BounceSpotlight</c>. This
    /// texture is the result of applying a gaussian blur to <c>ProductTexture</c>.
    /// </summary>
    private Texture BounceLightCookie { get; set; }


    /// <summary>
    /// This is where the shaders and textures are created for the first time.
    /// </summary>
    private void InitRendering()
    {
        // Don't unnecessarily recreate textures if light cookie size isn't default.
        if (BounceLightCookieSize != _PreviousLightCookieSize)
        {
            _PreviousLightCookieSize = BounceLightCookieSize;
        }
        CreateAllShaderTextures();
    }

    /// <summary>
    /// Instantiates all of the texture objects required by the compute shaders for
    /// fake bounce light.
    /// </summary>
    private void CreateAllShaderTextures()
    {
        DownscaledTexture = TextureUtilities.CreateShaderTexture(BounceLightCookieSize, BounceLightCookieSize);
        MultiplicandTexture = TextureUtilities.CreateShaderTexture(BounceLightCookieSize, BounceLightCookieSize);
        ProductTexture = TextureUtilities.CreateShaderTexture(BounceLightCookieSize, BounceLightCookieSize);
        BounceLightCookie = TextureUtilities.CreateShaderTexture(BounceLightCookieSize, BounceLightCookieSize);
        // Downscale the mask texture to the size of the bounce light cookie.
        var largeMaskTex = Texture.Load(FileSystem.Mounted, "materials/lightcookies/box_soft.vtex");
        largeMaskTex.DispatchDownscale(MultiplicandTexture);
    }



    [GameEvent.Client.Frame]
    public void OnClientFrame()
    {
        if (!EnableBounce)
        {
            return;
        }

        // If BounceLightCookieSize has changed, recreate all textures.
        HandleCookieSizeUpdate();

        // Get SourceTexture if it's null, or return if you can't get it.
        if (!EnsureSourceIsNotNull())
        {
            return;
        }

        //if (Game.LocalPawn is not Player ply)
        //    return;

        //if (ply.GetCurrentTheaterZone() is not CinemaZone zone)
        //    return;

        //var isClosest = zone.ProjectorEntity.ProjectionTexture == SourceTexture;

        //if (!isClosest)
        //    return;

        UpdateBounceLightCookie();
        if (BounceDebug >= 1)
        {
            DebugDrawProjectionInfo();
            if (BounceDebug >= 2)
            {
                DebugDrawShaderTextures();
            }
        }
    }

    private void HandleCookieSizeUpdate()
    {
        if (_PreviousLightCookieSize != BounceLightCookieSize)
        {
            CreateAllShaderTextures();
            BounceSpotlight.LightCookie = BounceLightCookie;
        }
        _PreviousLightCookieSize = BounceLightCookieSize;
    }

    /// <summary>
    /// If <c>SourceTexture</c> is null, tries to set it to the <c>LightCookie</c> of
    /// <c>Entity</c>, if <c>Entity</c> is a light that supports light cookies. 
    /// Returns true if source texture is not null.
    /// </summary>
    /// <returns></returns>
    private bool EnsureSourceIsNotNull()
    {
        if (SourceTexture != null)
        {
            return true;
        }
        SourceTexture = Entity switch
        {
            OrthoLightEntity ortho => ortho.LightCookie,
            SpotLightEntity spot => spot.LightCookie,
            _ => null
        };
        return SourceTexture != null;
    }

    private void UpdateBounceLightCookie()
    {
        /* 
         *  Here we daisy-chain three compute shaders to downscale, multiply, and blur the
         *  main projector texture in order to create a fake bounce light effect.
         *  I am CERTAIN that this is not the most efficient way to do this, but it works
         *  for now, and someone with more HLSL knowledge could probably do it all in one shader.
         */
        SourceTexture.DispatchDownscale(DownscaledTexture);
        DownscaledTexture.DispatchMultiply(MultiplicandTexture, ProductTexture);
        ProductTexture.DispatchGaussianBlur(BounceLightCookie);
    }

    /// <summary>
    /// Draw the positions of the projector and the bounce spotlight, as well as the line between them.
    /// </summary>
    private void DebugDrawProjectionInfo()
    {
        DebugOverlay.Sphere(Entity.Position, 5f, Color.Blue);
        DebugOverlay.Line(Entity.Position, BounceSpotlight.Position, Color.Yellow);
        DebugOverlay.Circle(BounceSpotlight.Position, BounceSpotlight.Rotation, BounceSpotlight.OuterConeAngle, Color.Red);
    }

    /// <summary>
    /// Draw on-screen all input and output textures used by the compute shaders.
    /// </summary>
    private void DebugDrawShaderTextures()
    {
        DebugOverlay.Texture(SourceTexture, new Vector2(0, 0));
        int texPosY = 0;
        DebugOverlay.Texture(DownscaledTexture, new Vector2(SourceTexture.Width, texPosY));
        texPosY += DownscaledTexture.Height;
        DebugOverlay.Texture(ProductTexture, new Vector2(SourceTexture.Width, texPosY));
        texPosY += ProductTexture.Height;
        DebugOverlay.Texture(BounceLightCookie, new Vector2(SourceTexture.Width, texPosY));
    }
}
