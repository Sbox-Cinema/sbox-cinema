using Sandbox;

using Conna.Inventory;

namespace Cinema;

[Title("Carriable"), Icon("luggage")]
public partial class Carriable : AnimatedEntity, IUse
{
    [Net]
    public string ViewModelPath { get; set; } = null;
    public BaseViewModel ViewModelEntity { get; protected set; }

    public IHandheldItem Item =>
        InternalItem.IsValid() ? InternalItem.Value as IHandheldItem : null;

    [Net]
    private NetInventoryItem InternalItem { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        PhysicsEnabled = true;
        UsePhysicsCollision = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;
    }

    public virtual void OnCarryStart(Entity carrier)
    {
        if (Game.IsClient)
            return;

        SetParent(carrier, true);
        Owner = carrier;
        EnableAllCollisions = false;
        EnableDrawing = false;
        UsePhysicsCollision = false;
        PhysicsEnabled = false;
    }

    public virtual void SimulateAnimator(CitizenAnimationHelper anim)
    {
        anim.HoldType = CitizenAnimationHelper.HoldTypes.Pistol;
        anim.Handedness = CitizenAnimationHelper.Hand.Both;
        anim.AimBodyWeight = 1.0f;
    }

    public virtual void OnCarryDrop(Entity dropper)
    {
        if (Game.IsClient)
            return;

        SetParent(null);
        Owner = null;
        EnableDrawing = true;
        EnableAllCollisions = true;
        UsePhysicsCollision = true;
        PhysicsEnabled = true;
    }

    /// <summary>
    /// This entity has become the active entity. This most likely
    /// means a player was carrying it in their inventory and now
    /// has it in their hands.
    /// </summary>
    public virtual void ActiveStart(Entity ent)
    {
        EnableDrawing = true;

        //
        // If we're the local player (clientside) create viewmodel
        // and any HUD elements that this weapon wants
        //
        if (IsLocalPawn)
        {
            DestroyViewModel();

            CreateViewModel();
            CreateHudElements();
        }
    }

    /// <summary>
    /// This entity has stopped being the active entity. This most
    /// likely means a player was holding it but has switched away
    /// or dropped it (in which case dropped = true)
    /// </summary>
    public virtual void ActiveEnd(Entity ent, bool dropped)
    {
        //
        // If we're just holstering, then hide us
        //
        if (!dropped)
        {
            EnableDrawing = false;
        }

        if (Game.IsClient)
        {
            DestroyViewModel();
            DestroyHudElements();
        }
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Game.IsClient && ViewModelEntity.IsValid())
        {
            DestroyViewModel();
            DestroyHudElements();
        }
    }

    /// <summary>
    /// Create the viewmodel. You can override this in your base classes if you want
    /// to create a certain viewmodel entity.
    /// </summary>
    public virtual void CreateViewModel()
    {
        Game.AssertClient();

        if (string.IsNullOrEmpty(ViewModelPath))
            return;

        ViewModelEntity = new BaseViewModel();
        ViewModelEntity.Position = Position;
        ViewModelEntity.Owner = Owner;
        ViewModelEntity.EnableViewmodelRendering = true;
        ViewModelEntity.SetModel(ViewModelPath);
    }

    public virtual void UpdateViewmodelCamera()
    {
        Game.AssertClient();
    }

    /// <summary>
    /// We're done with the viewmodel - delete it
    /// </summary>
    public virtual void DestroyViewModel()
    {
        ViewModelEntity?.Delete();
        ViewModelEntity = null;
    }

    public virtual void UpdateCamera()
    {
        Game.AssertClient();
    }

    /// <summary>
    /// Utility - return the entity we should be spawning particles from etc
    /// </summary>
    public virtual ModelEntity EffectEntity =>
        (ViewModelEntity.IsValid() && IsFirstPersonMode) ? ViewModelEntity : this;

    public bool OnUse(Entity user)
    {
        if (!IsUsable(user)) return false;

        return false;
    }

    public virtual bool IsUsable(Entity user)
    {
        return Owner == null;
    }

    public virtual void CreateHudElements() { }

    public virtual void DestroyHudElements() { }

    public virtual void AdjustInput() { }

    public void SetWeaponItem(IHandheldItem item)
    {
        InternalItem = new NetInventoryItem(item);
    }
}
