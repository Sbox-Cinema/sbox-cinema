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
    public virtual bool ToggleUse => false;

    public UI.NpcHeadText HeadText { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        SetModel("models/citizen/citizen.vmdl");
        SetupPhysicsFromAABB(PhysicsMotionType.Keyframed, new Vector3(-16, -16, 0), new Vector3(16, 16, 72));
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();

        HeadText = new UI.NpcHeadText(this);
    }

    public virtual bool OnUse(Entity user)
    {
        if (user is not Player player) return false;
        TriggerOnClientUse(player);
        return false;
    }

    [ClientRpc]
    private void TriggerOnClientUse(Player player)
    {
        OnClientUse(player);
    }

    public virtual void OnClientUse(Player player)
    {

    }

    public virtual bool IsUsable(Entity user)
    {
        return false;
    }

    public virtual void OnStopUse(Entity user)
    {
        if (user is not Player player) return;
        TriggerOnClientStopUse(player);
    }

    [ClientRpc]
    private void TriggerOnClientStopUse(Player player)
    {
        OnClientStopUse(player);
    }

    public virtual void OnClientStopUse(Player player)
    {

    }


}
