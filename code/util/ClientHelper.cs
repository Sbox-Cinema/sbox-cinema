using Cinema.player.bots;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;
public static class ClientHelper
{
    public static CinemaBot GetBot(this IClient client)
    {
        if (!client.IsBot)
            return null;

        var botId = client.GetBotId();
        return BotManager.Get(botId);
    }

    public static int GetBotId(this IClient client)
    {
        return client.GetInt("botId");
    }

    public static void SetBotId(this IClient client, int botId)
    {
        client.SetInt("botId", botId);
    }

    public static CinemaZone GetNearestZone(this IClient client)
    {
        if (client.Pawn is not Player ply)
            return null;

        return ply.GetCurrentTheaterZone();
    }

    public static IClient FindById(int clientId)
    {
        IClient client = null;
        if (clientId > 0)
        {
            client = Entity.FindByIndex(clientId) as IClient;
        }
        return client;
    }
}
