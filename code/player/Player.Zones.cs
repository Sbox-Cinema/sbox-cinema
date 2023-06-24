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

    public void EnterZone(CinemaZone zone)
    {
        if (Zones.Contains(zone))
        {
            return;
        }
        Zones.Add(zone);
    }

    public void ExitZone(CinemaZone zone)
    {
        Zones.Remove(zone);
    }

    [ConCmd.Server("player.zones.dump")]
    public static void DumpZonesCmd()
    {
        if (ConsoleSystem.Caller.Pawn is not Player ply)
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
