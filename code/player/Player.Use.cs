using Sandbox;
using Sandbox.UI;

namespace Cinema;

public partial class Player
{
    /// <summary>
    /// Entity the player is currently using via their interaction key.
    /// </summary>
    [Net]
    public Entity Using { get; protected set; }
    public Vector3 UsePoint { get; protected set; }
    public float InteractRange => 85;
    public bool HideUsePrompt { get; set; } = false;
    public bool IsUsingSomething => Using != null;

    public bool IsUseDisabled()
    {
        var heldItemIsUsable = ActiveChild is IUse use && use.IsUsable(this);
        return IsMenuOpen || heldItemIsUsable;
    }

    protected virtual void TickPlayerUse()
    {
        PlayerUseServer();
    }

    internal void PlayerUseServer()
    {
        // This is serverside only
        if (!Game.IsServer)
            return;

        // Turn prediction off
        using (Prediction.Off())
        {
            if (Input.Pressed("use"))
            {
                if (IsToggleUseEntity(Using))
                {
                    StopUsing();
                    return;
                }

                Using = FindUsable();

                if (Using == null && !IsUseDisabled())
                {
                    UseFail();
                    return;
                }

                if (Using is not IUse usable) return;

                if (usable.OnUse(this))
                    return;

                StopUsing();
                return;
            }

            if (!Using.IsValid())
                return;

            // Moved too far away while using
            if (UsePoint.DistanceSquared(EyePosition) > InteractRange * InteractRange)
            {
                StopUsing();
                return;
            }

            if (IsToggleUseEntity(Using)) return;

            if (!Input.Down("use"))
            {
                StopUsing();
                return;
            }

            if (Using is IUse use && use.OnUse(this))
                return;

            if (Using is ICinemaUse cinemaUse && cinemaUse.ToggleUse)
                return;

            StopUsing();
        }
    }

    /// <summary>
    /// Find a usable entity for this player to use
    /// </summary>
    public virtual Entity FindUsable(bool includeUnable = false)
    {
        if (IsUseDisabled())
            return null;

        // First try a direct 0 width line
        var tr = Trace
            .Ray(EyePosition, EyePosition + EyeRotation.Forward * InteractRange)
            .Ignore(this)
            .Run();

        // See if any of the parent entities are usable if we ain't.
        var ent = tr.Entity;
        while (ent.IsValid() && !IsValidUseEntity(ent, includeUnable))
        {
            ent = ent.Parent;
        }

        // Nothing found, try a wider search
        if (!IsValidUseEntity(ent))
        {
            tr = Trace
                .Ray(EyePosition, EyePosition + EyeRotation.Forward * InteractRange)
                .Radius(15)
                .Ignore(this)
                .EntitiesOnly()
                .Run();

            // See if any of the parent entities are usable if we ain't.
            ent = tr.Entity;
            while (ent.IsValid() && !IsValidUseEntity(ent, includeUnable))
            {
                ent = ent.Parent;
            }

            // Still no good? Bail.
            if (!IsValidUseEntity(ent, includeUnable))
                return null;
        }

        UsePoint = tr.HitPosition;
        return ent;
    }

    /// <summary>
    /// Returns if the entity is a valid usable entity
    /// </summary>
    protected bool IsValidUseEntity(Entity e, bool includeUnable = false)
    {
        if (e == null)
            return false;
        if (e is not IUse use)
            return false;
        if (!use.IsUsable(this))
        {
            if (includeUnable && use is ICinemaUse cinemaUse)
            {
                return cinemaUse.ShowCannotUsePrompt;
            }
            return false;
        }

        return true;
    }

    protected static bool IsToggleUseEntity(Entity e)
    {
        return e is not null && e is ICinemaUse cinemaUse && cinemaUse.ToggleUse;
    }

    /// <summary>
    /// Player tried to use something but there was nothing there.
    /// Tradition is to give a disappointed boop.
    /// </summary>
    protected virtual void UseFail()
    {
        PlaySound("player_use_fail");
    }

    /// <summary>
    /// Tells the player pawn to stop using an entity.
    /// Used internally but can also be called from the entity itself or an other external method.
    /// Returns true if the entity was successfully stopped being used.
    /// Use with caution.
    /// </summary>
    public bool StopUsing(Entity entity = null)
    {
        if (entity == null)
        {
            entity = Using;
        }

        if (!IsUsingSomething)
        {
            Log.Warning($"Not using entity: {entity}");
            return false;
        }

        if (entity != Using)
        {
            Log.Warning($"Using another entity: {Using} but told to stop using {entity}");
            return false;
        }

        var canStopUsing = true;

        if (Using is ICinemaUse cinemaUse)
        {
            canStopUsing = cinemaUse.OnStopUse(this);
        }

        if (canStopUsing) Using = null;

        return canStopUsing;
    }

}
