using Sandbox;

namespace Cinema;

public class AstroTurferBot : CinemaBot
{
    public CinemaChair AssignedSeat { get; set; }
    private TimeUntil StopLookTime { get; set; }
    private Vector3 RandomLookPos { get; set; }

    [ConCmd.Admin("bot.astro.spawn")]
    public static void SpawnAstroTurfer()
    {
        if (ConsoleSystem.Caller?.Pawn is not Player ply)
            return;

        var capsule = Capsule.FromHeightAndRadius(64f, 32f);
        var tr = Trace
            .Capsule(capsule, ply.AimRay, 100f)
            .WithTag("chair")
            .Run();

        if (tr.Entity is not CinemaChair chair || chair.IsOccupied)
            return;

        var bot = new AstroTurferBot();
        bot.AssignedSeat = chair;
    }

    public override void OnRespawn()
    {
        base.OnRespawn();

        AssignedSeat.OnUse(Client.Pawn as Player);
    }

    public override void BuildInput()
    {
        base.BuildInput();

        if (Client.Pawn is not Player ply)
            return;

        var currentZone = ply.GetCurrentTheaterZone();

        var nearestPlayer = FindNearestPlayer(100f);
        // If there's a nearby player, look at them.
        if (nearestPlayer.IsValid())
        {
            LookAt(nearestPlayer.EyePosition + Vector3.Zero.WithZ(8f));
        }
        // Otherwise, look at the screen if media is playing.
        else if (currentZone?.MediaController?.CurrentMedia != null)
        {
            var screenPos = ply.GetCurrentTheaterZone().ProjectorEntity.ScreenPosition;
            LookAt(screenPos);
        }
        // Otherwise, look somewhere random.
        else
        {
            LookRandomly();
        }
    }

    private void LookRandomly()
    {
        if (Client.Pawn is not Player ply)
            return;

        if (!StopLookTime)
        {
            LookAt(RandomLookPos);
            return;
        }

        StopLookTime = Game.Random.Float(0.25f, 15f);
        var frontPos = ply.Position + ply.Rotation.Forward * 200f + Vector3.Zero.WithZ(64f);
        var frontBox = BBox.FromPositionAndSize(frontPos, 180f);
        RandomLookPos = frontBox.RandomPointInside;
        LookAt(RandomLookPos);
    }
}
