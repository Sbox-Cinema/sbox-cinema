using Cinema.player.bots;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class CinemaBot : Bot
{
    public CinemaBot()
    {
        Id = BotManager.AddBot(this);
        Client.SetBotId(Id);
        ClothingString = RandomOutfit.Generate().Serialize();
    }

    public virtual void OnRespawn()
    {
        
    }

    public int Id { get; init; }
    public Vector3? SpawnPosition { get; set; }
    public string ClothingString { get; set; }
}
