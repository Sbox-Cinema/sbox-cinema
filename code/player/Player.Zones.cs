using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class Player
{
    [Net]
    public IList<CinemaZone> Zones { get; set; }

    public List<CinemaZone> GetZones() => Zones.ToList();
    public void IsInZone(CinemaZone zone) => Zones.Contains(zone);
    public event EventHandler<CinemaZone> ZoneEntered;
    public event EventHandler<CinemaZone> ZoneExited;

    public CinemaZone GetCurrentZone()
        => Zones
            .OrderBy(x => x.Position.Distance(Position))
            .FirstOrDefault();

    public CinemaZone GetCurrentTheaterZone()
        => Zones
            .Where(z => z.IsTheaterZone)
            .OrderBy(x => x.Position.Distance(Position))
            .FirstOrDefault();

    public async void EnterZone(CinemaZone zone)
    {
        if (Zones.Contains(zone))
        {
            return;
        }
        Zones.Add(zone);
        if (!Client.IsBot && zone.IsTheaterZone)
        {
            // Wait a frame to ensure that the added zone has been replicated to the client
            // before sending the RPCs to initialize on the client.
            await GameTask.DelaySeconds(Time.Delta);
            zone.ProjectorEntity.ClientInitialize(To.Single(Client));
            zone.MediaController.ClientPlayMedia(To.Single(Client));
        }
        ZoneEntered?.Invoke(this, zone);
    }

    public void ExitZone(CinemaZone zone)
    {
        Zones.Remove(zone);
        if (!Client.IsBot && zone.IsTheaterZone)
        {
            zone.ProjectorEntity.ClientCleanup(To.Single(Client));
            zone.MediaController.ClientStopMedia(To.Single(Client));
        }
        ZoneExited?.Invoke(this, zone);
    }

    [ConCmd.Server("player.zones.dump.server")]
    public static void DumpZonesServer()
    {
        if (ConsoleSystem.Caller?.Pawn is not Player ply)
            return;

        ply.DumpZones();
    }

    [ConCmd.Client("player.zones.dump.client")]
    public static void DumpZonesClient()
    {
        if (Game.LocalPawn is not Player ply)
            return;

        ply.DumpZones();
    }

    public void DumpZones()
    {
        var zones = GetZones();
        if (!zones.Any())
        {
            Log.Info("No zones found.");
            return;
        }
        foreach(var zone in GetZones())
        {
            Log.Info($"{zone.Name}, Media Controller: \"{zone?.MediaController?.Name ?? "null"}\", Projector Entity: \"{zone?.ProjectorEntity?.Name ?? "null"}\"");
        }
    }
}
