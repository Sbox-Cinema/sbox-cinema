using Cinema.player.bots;
using Sandbox;
using System.Linq;

namespace Cinema;

public class CinemaBot : Bot
{
    public int Id { get; init; }
    public Vector3? SpawnPosition { get; set; }
    public string ClothingString { get; set; }

    public CinemaBot()
    {
        Id = BotManager.AddBot(this);
        Client.SetBotId(Id);
        ClothingString = RandomOutfit.Generate().Serialize();
    }

    public virtual void OnRespawn()
    {
        
    }

    protected void LookAt(Vector3 worldPos)
    {
        if (Client.Pawn is not Player ply)
            return;

        var eyePos = ply.Position + Vector3.Zero.WithZ(64f);
        ply.EyeRotation = Rotation.LookAt((worldPos - eyePos).Normal);
        ply.LookInput = ply.EyeRotation.Angles().WithRoll(0);
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
