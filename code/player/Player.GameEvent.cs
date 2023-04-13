using Sandbox;
using Sandbox.Diagnostics;
using System.Linq;

namespace Cinema;

public partial class Player
{
    static string realm = Game.IsServer ? "server" : "client";
    static Logger eventLogger = new Logger($"player/GameEvent/{realm}");

    public void RunGameEvent(string eventName)
    {
        eventName = eventName.ToLowerInvariant();
        // maybe just use Event.Run
        Components
            .Get<PlayerBodyController>()
            ?.Mechanics.ToList()
            .ForEach(x => x.OnGameEvent(eventName));

        eventLogger.Trace($"OnGameEvent ({eventName})");
    }
}
