using Cinema.player.bots;
using Sandbox;
using System.Linq;

namespace Cinema;

public class CinemaBot : Bot
{
    public int Id { get; init; }
    public Vector3? SpawnPosition { get; set; }
    public string ClothingString { get; set; }
    private TimeUntil StopLookTime { get; set; }
    private Vector3 RandomLookPos { get; set; }

    public CinemaBot()
    {
        Id = BotManager.AddBot(this);
        Client.SetBotId(Id);
        ClothingString = RandomOutfit.Generate().Serialize();
    }

    public virtual void OnRespawn()
    {
        
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

    protected void LookAt(Vector3 worldPos)
    {
        if (Client.Pawn is not Player ply)
            return;

        var eyePos = ply.Position + Vector3.Zero.WithZ(64f);
        ply.EyeRotation = Rotation.LookAt((worldPos - eyePos).Normal);
        ply.LookInput = ply.EyeRotation.Angles().WithRoll(0);
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

    protected Player FindNearestPlayer(float radius)
    {
        var players = Entity.All.OfType<Player>();
        var nearestPlayer = players
            .Where(p => !p.Client.IsBot)
            .Where(p => p.Position.Distance(Client.Pawn.Position) <= radius)
            .OrderBy(p => p.Position.Distance(Client.Pawn.Position))
            .FirstOrDefault();
        return nearestPlayer;
    }
}
