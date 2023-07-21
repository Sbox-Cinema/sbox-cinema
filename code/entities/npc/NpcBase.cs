using System.ComponentModel;
using Sandbox;

namespace Cinema;

public partial class NpcBase : AnimatedEntity, ICinemaUse
{

    /// <summary>
    /// Name of the NPC, displayed over head
    /// </summary>
    new public virtual string Name => "NPC";
    /// <summary>
    /// Description of the NPC, displayed over head
    /// </summary>
    public virtual string Description => "";
    /// <summary>
    /// The name of an outfit resource that will be worn by this NPC when it spawns.
    /// If null, the NPC will be naked.
    /// </summary>
    public virtual string Uniform => "usher";
    /// <summary>
    /// Whether using the NPC is toggleable
    /// </summary>
    public virtual bool ToggleUse => false;

    public UI.NpcHeadText HeadText { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/citizen/citizen.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
        var uniform = JobUniform.Get(Uniform);
        if (uniform != null)
        {
            var uniformClothing = uniform.GetOutfit(null);
            uniformClothing.DressEntity(this);
        }
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();

        HeadText = new UI.NpcHeadText(this);
    }


    /// <summary>
    /// Whether this NPC is usable or not
    /// </summary>
    /// <param name="user">The player who is using</param>
    /// <returns></returns>
    public virtual bool IsUsable(Entity user)
    {
        return false;
    }

    /// <summary>
    /// Called on the server when the NPC is used by a player
    /// </summary>
    /// <param name="user"></param>
    /// <returns>If the player can continue to use the NPC</returns>
    public virtual bool OnUse(Entity user)
    {
        if (user is not Player player) return false;
        TriggerOnClientUse(To.Single(user.Client), player);
        return true;
    }

    // Internal: Triggers OnClientUse
    [ClientRpc]
    private void TriggerOnClientUse(Player player)
    {
        OnClientUse(player);
    }

    /// <summary>
    /// Called on the client when the NPC is used
    /// </summary>
    /// <param name="player">The player who uses the NPC</param>
    public virtual void OnClientUse(Player player)
    {

    }

    /// <summary>
    /// Called on the server when the player stops using the NPC
    /// </summary>
    /// <param name="user">The player</param>
    public virtual bool OnStopUse(Entity user)
    {
        if (user is not Player player) return true;
        TriggerOnClientStopUse(player);

        return true;
    }

    // Internal, trigger OnClientStopUse on the client
    [ClientRpc]
    private void TriggerOnClientStopUse(Player player)
    {
        OnClientStopUse(player);
    }

    /// <summary>
    /// Called on the client when the player stops using the NPC
    /// </summary>
    /// <param name="player">The player</param>
    public virtual void OnClientStopUse(Player player)
    {

    }


}
