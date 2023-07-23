using Cinema.player.bots;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class AstroTurferBot : CinemaBot
{
    public CinemaChair AssignedSeat { get; set; }

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

    public override void Tick()
    {
        if (Client.Pawn is not Player ply)
            return;
    }
}
