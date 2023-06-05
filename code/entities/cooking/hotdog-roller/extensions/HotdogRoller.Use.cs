using Sandbox;
using System.Numerics;

namespace Cinema;

public partial class HotdogRoller
{
    /// <summary>
    /// Whether this Hotdog Roller is usable or not
    /// </summary>
    /// <param name="user">The player who is using</param>
    /// <returns>If this is useable</returns>
    public virtual bool IsUsable(Entity user)
    {
        return true;
    }

    /// <summary>
    /// Called on the server when the Hotdog Roller is used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the Hotdog Roller</returns>
    public virtual bool OnUse(Entity user)
    {
        if (Game.IsClient) return false;

        TryInteractions(user);

        Input.Clear("use"); // Why?

        return false;
    }

    /// <summary>
    /// Called on the server when the Hotdog Roller is stopped being used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the Hotdog Roller</returns>
    public void OnStopUse(Entity user)
    {
        
    }

    private void TryInteractions(Entity user)
    {
        foreach (var volume in InteractionVolumes)
        {
            var name = volume.Key;
            var bounds = volume.Value;

            if (bounds.Trace(user.AimRay, 1000, out float distance))
            {
                TryInteraction(name);
            }
            
        }
    }
}
