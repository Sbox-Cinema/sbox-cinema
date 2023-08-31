using Sandbox;
using System.Linq;

namespace Cinema;

public partial class CigaretteMachine
{
    public string UseText { get; set; } = "Use Cigarette Machine";

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

        HandleUse(user);

        return false;
    }

    /// <summary>
    /// Called on the server when the entity is stopped being used by a player
    /// </summary>
    /// <param name="user"></param>
    public void OnStopUse(Entity user)
    {

    }

    /// <summary>
    /// Called on the server when the player is trying to use something interactable
    /// </summary>
    /// <param name="player"></param>
    public void HandleUse(Entity player)
    {
        var interactable = Interactables.FirstOrDefault(x => x.CanRayTrigger(player.AimRay).Hit);

        interactable?.Trigger(player as Player);
    }
}
