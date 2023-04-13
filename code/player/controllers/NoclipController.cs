namespace Cinema;

using Sandbox;

[Library]
public class NoclipController : PlayerController, ISingletonComponent
{
    public override void Simulate(IClient cl)
    {
        var pl = cl.Pawn as Player;

        var fwd = pl.MoveInput.x.Clamp(-1f, 1f);
        var left = pl.MoveInput.y.Clamp(-1f, 1f);
        var rotation = pl.LookInput.ToRotation();

        var vel = (rotation.Forward * fwd) + (rotation.Left * left);

        if (Input.Down(InputButton.Jump))
        {
            vel += Vector3.Up * 1;
        }

        vel = vel.Normal * 2000;

        if (Input.Down(InputButton.Run))
            vel *= 5.0f;

        if (Input.Down(InputButton.Duck))
            vel *= 0.2f;

        pl.Velocity += vel * Time.Delta;

        if (pl.Velocity.LengthSquared > 0.01f)
        {
            pl.Position += pl.Velocity * Time.Delta;
        }

        pl.Velocity = pl.Velocity.Approach(0, pl.Velocity.Length * Time.Delta * 5.0f);

        pl.EyeRotation = rotation;
        pl.GroundEntity = null;
        pl.BaseVelocity = Vector3.Zero;
    }

    public override void FrameSimulate(IClient cl)
    {
        var pl = cl.Pawn as Player;
        pl.EyeRotation = pl.LookInput.ToRotation();
        pl.EyeLocalPosition = Vector3.Up * 64;
    }
}
