using Sandbox;
using System.Collections.Generic;

namespace Cinema.player.bots
{
    public static class BotManager
    {
        public static int NextId => Bots.Count;
        private static readonly List<CinemaBot> Bots = new();

        public static int AddBot(CinemaBot bot)
        {
            Bots.Add(bot);
            return Bots.Count - 1;
        }

        public static CinemaBot Get(int id)
        {
            if (id < 0 || id >= Bots.Count)
            {
                Log.Info($"Tried to get invalid bot ID: {id}. Bot count: {Bots.Count}");
                return null;
            }

            return Bots[id];
        }

        public static IEnumerable<CinemaBot> GetAll() => Bots;
    }
}
