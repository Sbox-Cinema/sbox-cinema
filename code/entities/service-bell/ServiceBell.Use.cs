using Sandbox;

namespace Cinema;

public partial class ServiceBell
{
    public string UseText { get; set; } = "Ring Service Bell";

    /// <summary>
    /// Whether this entity is usable or not
    /// </summary>
    /// <param name="user">The player who is using</param>
    /// <returns>If this is useable</returns>
    public virtual bool IsUsable(Entity user)
    {
        return true;
    }

    /// <summary>
    /// Called on the server when the entity is used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the entity</returns>
    public virtual bool OnUse(Entity user)
    {
        if (Game.IsClient) return false;

        Log.Info("Ringing service bell...");

        return false;
    }

    /// <summary>
    /// Called on the server when the entity is stopped being used by a player
    /// </summary>
    /// <param name="user"></param>
    public void OnStopUse(Entity user)
    {

    }
}
