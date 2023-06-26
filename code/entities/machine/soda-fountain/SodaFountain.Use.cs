using Sandbox;
using Sandbox.util;

namespace Cinema;

public partial class SodaFountain 
{
    public string UseText { get; set; } = "Use Soda Fountain";

    /// <summary>
    /// Whether this Soda Fountain is usable or not
    /// </summary>
    /// <param name="user">The player who is using</param>
    /// <returns>If this is useable</returns>
    public virtual bool IsUsable(Entity user)
    {
        return true;
    }

    /// <summary>
    /// Called on the server when the Soda Fountain is used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the Hotdog Roller</returns>
    public virtual bool OnUse(Entity user)
    {
        if (Game.IsClient) return false;

        HandleUse(user);

        return false;
    }

    /// <summary>
    /// Called on the server when the Soda Fountain is stopped being used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the Hotdog Roller</returns>
    public void OnStopUse(Entity user)
    {

    }
}
