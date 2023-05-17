using Sandbox;

namespace Cinema.UI;
public partial class Placement
{
    private string Text = "";
    private bool Open { get; set; } = false;
    public Placement(Transform? transform, string text)
    {
        Text = text;

        Transform = transform.Value;

        Rotation = Rotation.FromYaw(90.0f);
        Rotation = Rotation.FromPitch(90.0f);
        Position += Vector3.Up * 1.25f;
    }
    public void ShouldOpen(bool open)
    {
        Open = open;

        StateHasChanged();
    }
    public override void Tick()
    {
        base.Tick();

        if (Game.LocalPawn is Player player)
        {
            TraceResult tr = Trace.Ray(player.AimRay, 1024)
                .WithTag("interactable")
                .Ignore(player)
                .Run();

            if (tr.Hit)
            {
                ShouldOpen(true);
            }
            else
            {
                ShouldOpen(false);
            }
        }
    }
}
