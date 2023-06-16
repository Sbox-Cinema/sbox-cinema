using Sandbox;

namespace Cinema.UI;

public partial class NameTag 
{
    public Player Player { get; set; }

    public NameTag(Player player)
    {
        Player = player;
    }

    public override void Tick()
    {
        if (!Player.IsValid)
        {
            Delete();
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
