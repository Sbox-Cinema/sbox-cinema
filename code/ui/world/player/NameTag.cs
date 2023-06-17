using Sandbox;

namespace Cinema.UI;

public partial class NameTag 
{
    /// <summary>
    /// Fade distance of name tags, for all players
    /// </summary>
    [ConVar.Replicated("nametag.distance")]
    public static float FadeDistance { get; set; } = 500f;

    public Player Player { get; set; }

    public NameTag(Player player)
    {
        Player = player;
    }

    public override void Tick()
    {
        var DistanceThreshold = Player.Position.Distance(Camera.Position) > FadeDistance;

        if (!Player.IsValid)
        {
            Delete();
            return;
        }

        if (DistanceThreshold)
        {
            Title.Style.Opacity = 0;
            return;
        }

        if (Player.IsFirstPersonMode)
        {
            Title.Style.Opacity = 0;
            return;
        }

        if (Player.ActiveController is ChairController)
        {
            Title.Style.Opacity = 0;
            return;
        }

        Title.Style.Opacity = 1;
        Position = Player.GetBoneTransform("head").Position + Vector3.Up * 20;
        Rotation = Rotation.LookAt(Camera.Rotation.Forward * -1.0f, Vector3.Up);
    }
}
