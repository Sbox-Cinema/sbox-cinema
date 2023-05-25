using Sandbox;
using System;

namespace Cinema.UI;
public partial class Tooltip
{
    private string Text = "";
    private Entity Entity { get; set; }
    private bool Open { get; set; } = false;
    public Tooltip(Entity ent, string text)
    {
        Text = text;

        Transform = ent.Transform;
        Position += Vector3.Up * 24.0f;
    }
    public void ShouldOpen(bool open)
    {
        Open = open;

        StateHasChanged();
    }

    public void SetText(string text)
    {
        Text = text;
    }

    public override void Tick()
    {
        base.Tick();
   /*
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
   */
            Rotation = Rotation.LookAt(Camera.Rotation.Forward * -1.0f, Vector3.Up);
     //   }
    }

    protected override int BuildHash()
    {
        return HashCode.Combine(Text);
    }
}
