using System;
using System.Runtime.CompilerServices;
using CinemaTeam.Plugins.Video;
using Sandbox;
using Sandbox.UI;

namespace Cinema;

public partial class ProjectorEntity
{
    [ConVar.Client("projector.cookie.margin")]
    public static float ProjectorLightCookieMargin { get; set; } = 0.93f;
    [ConVar.Client("projector.cookie.debug")]
    public static int ProjectorLightCookieDebug { get; set; } = 0;

    /// <summary>
    /// The media we want to be playing (but might not be)
    /// </summary>
    public IVideoPresenter CurrentMedia { get; protected set; }
    private SpotLightEntity ProjectionLight { get; set; }
    public Texture InputTexture { get; set; }
    protected Texture LastInputTexture { get; set; }
    protected Texture LightCookieTexture { get; set; }

    [Net]
    public Vector3 ScreenPosition { get; set; }
    [Net]
    public float ScreenDistance { get; set; }
    protected bool ShouldRemakeLight { get; set; }



    private void InitializeProjection()
    {
        var outerConeAngle = 60f;
        var innerConeAngle = 45f;
        if (ProjectionLight.IsValid())
        {
            outerConeAngle = ProjectionLight.OuterConeAngle;
            innerConeAngle = ProjectionLight.InnerConeAngle;
        }
        ProjectionLight?.Delete();

        float largestDimension = Math.Max(InputTexture?.Width ?? 320, InputTexture?.Height ?? 320);
        var margin = Math.Clamp(ProjectorLightCookieMargin, 0.2f, 1.0f);
        largestDimension /= margin;
        LightCookieTexture = TextureUtilities.CreateShaderTexture((int)largestDimension, (int)largestDimension);

        ProjectionLight = new SpotLightEntity
        {
            Parent = this,
            Position = Position,
            Rotation = Rotation,
            LightCookie = LightCookieTexture,
            Brightness = 20.0f,
            Range = 1024.0f,
            OuterConeAngle = outerConeAngle,
            InnerConeAngle = innerConeAngle,
            DynamicShadows = true,
            FogStrength = 1.0f,
            Transmit = TransmitType.Always,
        };

        if (InputTexture != null)
            Log.Trace($"{Name} - Input texture: {InputTexture.Width}x{InputTexture.Height}");

        Log.Trace($"{Name} - Light cookie texture: {LightCookieTexture.Width}x{LightCookieTexture.Height}");

        ProjectionLight.UseFog();
        ProjectionLight.Components.Create<FakeBounceLight>();
    }

    public bool CanSeeProjector(Vector3 pos)
    {
        // Orphaned projectors can't play media, so return null.
        if (Zone == null)
            return false;

        var inside = Zone.WorldSpaceBounds.Contains(pos);
        return inside;
    }

    [GameEvent.Tick.Server]
    protected void OnServerTick()
    {
        UpdateScreenPositionAndDistance();
    }

    [GameEvent.Tick.Client]
    protected virtual void UpdateClientProjection()
    {
        if (!ProjectionLight.IsValid())
            return;

        UpdateProjectorAngles();

        // If the projection texture changed, remake the projector and bounce lights.
        if (InputTexture != LastInputTexture)
        {
            Log.Trace($"{Name} - Projection texture changed.");
            InitializeProjection();
        }
        LastInputTexture = InputTexture;
    }

    [GameEvent.Client.Frame]
    protected virtual void OnFrame()
    {
        if (InputTexture == null || LightCookieTexture == null)
            return;

        LightCookieTexture.DispatchColorPad(
            fromTex: InputTexture, 
            padColor: ProjectorLightCookieDebug > 0 ? Color.Magenta : Color.Black, 
            padRatio: ProjectorLightCookieMargin,
            fitAspectRatio: ProjectionSize.x / ProjectionSize.y
            );

        if (ProjectorLightCookieDebug > 0)
        {
            var inputSize = DebugOverlayTexture(InputTexture, Vector2.Zero);
            DebugOverlayTexture(LightCookieTexture, new Vector2(inputSize, 0));
        }

        float DebugOverlayTexture(Texture texture, Vector2 position)
        {
            Vector2 overlaySize;
            var aspectRatio = (float)texture.Width / (float)texture.Height;
            overlaySize = new Vector2(200 * aspectRatio, 200);
            DebugOverlay.Texture(texture, new Rect(position, overlaySize));
            return overlaySize.x;
        }
    }

    protected void UpdateScreenPositionAndDistance()
    {
        const float maxDistance = 5000f;

        // Trace forward from the projector light to find the surface it projects on to.
        var traceStart = Position;
        var traceEnd = Position + Rotation.Forward * maxDistance;
        var tr = Trace.Ray(traceStart, traceEnd)
            .StaticOnly()
            .Run();

        // If nothing hit, we act as if the screen is somewhere far in front of the projector.
        ScreenPosition = tr.Hit
            ? tr.HitPosition
            : traceEnd;

        ScreenDistance = tr.Hit
            ? tr.Distance
            : maxDistance;
    }

    protected void UpdateProjectorAngles()
    {
        var largestSideSize = MathF.Max(ProjectionSize.x, ProjectionSize.y);
        var smallestSideSize = MathF.Min(ProjectionSize.x, ProjectionSize.y);
        var aspectRatio = largestSideSize / smallestSideSize;
        // Scale up the spotlight to try to fit the all corners of the screen,
        // then third (!?!?) it for... reasons?!?
        var spotLightRadius = largestSideSize * aspectRatio / 3f;
        // Pretty much gets the visual angle/angular diameter of the screen as seen by the projector,
        // assuming you look at it head-on. Good enough approximation for now.
        var angle = MathF.Atan(spotLightRadius / ScreenDistance);
        var outerConeAngle = MathX.RadianToDegree(angle);

        ProjectionLight.OuterConeAngle = outerConeAngle;
        // Just make the inner cone angle smaller.
        ProjectionLight.InnerConeAngle = ProjectionLight.OuterConeAngle * 0.5f;
    }

    private void ProjectCurrentMedia()
    {
        if (CurrentMedia == null)
        {
            ProjectionLight.Delete();
            return;
        }
        if (!Game.LocalPawn.IsValid()) return;

        if (CurrentMedia?.Texture == null)
        {
            Log.Info($"{Name} - Media texture is null.");
            return;
        }
        if (CurrentMedia.Texture.Width == 1 && CurrentMedia.Texture.Height == 1)
        {
            Log.Info($"{Name} - Video is blank due to some kind of issue.");
        }

        InputTexture = CurrentMedia.Texture;
        InitializeProjection();
    }

    protected void CleanupProjection()
    {
        LightCookieTexture?.Dispose();
    }
}
