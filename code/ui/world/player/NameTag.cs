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
        var IsOutOfRange = Player.Position.Distance(Camera.Position) > FadeDistance;

        if (!Player.IsValid)
        {
            Delete(true);
            return;
        }

        if ((Game.LocalPawn as Player)?.ActiveController is ChairController && Player.ActiveController is ChairController)
        {
            Title.Style.Opacity = 0;
        }
        else
        {
            Title.Style.Opacity = 1;
        }

        if ((Game.LocalPawn as Player).IsFirstPersonMode || IsOutOfRange)
        {
            Title.Style.Opacity = 0;
        }
        else
        {
            Title.Style.Opacity = 1;
        }


        Position = Player.GetBoneTransform("head").Position + Vector3.Up * 20;
        Rotation = Rotation.LookAt(Camera.Rotation.Forward * -1.0f, Vector3.Up);
    }
}
