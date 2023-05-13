using Sandbox;
using System;

namespace Cinema;

public partial class Armrest : BaseNetworkable
{
    public enum States : int
    {
        Lowered,
        Raising,
        Raised,
        Lowering
    }

    public enum Sides : int
    {
        Left,
        Right
    }

    [Net]
    public CinemaChair Chair { get; set; }
    [Net]
    public ModelEntity HeldEntity { get; set; }
    [Net]
    public States State { get; set; } = States.Lowered;
    [Net]
    public Sides Side { get; set; }

    public string CupholderBoneName => Side switch
    {
        Sides.Left => "cupholder_l",
        _ => "cupholder_r"
    };

    private Transform GetCupholderTransform()
        => Chair.GetBoneTransform(CupholderBoneName);

    public string ArmrestToggleParameter => Side switch
    {
        Sides.Left => "toggle_left_armrest",
        _ => "toggle_right_armrest"
    };

    public void HandleAnimTag(string tag)
    {
        if (tag.Contains("launch"))
        {
            // Default to launching HeldEntity back and up a bit.
            var launchDirection = (Chair.Rotation.Backward + Chair.Rotation.Up).Normal;
            LaunchHeldEntity(launchDirection, 250f, 0.2f);
            State = States.Raised;
        }
        else if (tag.Contains("lowered"))
        {
            State = States.Lowered;
        }
    }

    public void Toggle()
    {
        var originalValue = Chair.GetAnimParameterBool(ArmrestToggleParameter);
        Chair.SetAnimParameter(ArmrestToggleParameter, !originalValue);
        // If toggle_seat was true but now is false, we are now lowering the seat.
        State = originalValue
            ? States.Lowering
            : States.Raising;
    }

    /// <summary>
    /// Will set the entity contained within the cupholder of this armrest. Will
    /// return without doing anything if the <c>State</c> is not lowered. Will 
    /// launch any entity that is already held.
    /// </summary>
    /// <param name="entity">The entity that shall be held in the cupholder.</param>
    public bool TryHoldEntity(ModelEntity entity)
    {
        if (State != States.Lowered)
        {
            return false;
        }
        if (HeldEntity != null)
        {
            var sideVec = (Vector3)Vector2.Random * 0.5f;
            // Just pop the held entity up and out out of the cupholder.
            LaunchHeldEntity(Chair.Rotation.Up + sideVec, 150f);
        }
        HoldEntity(entity);
        return true;
    }

    private void HoldEntity(ModelEntity entity)
    {
        HeldEntity = entity;
        entity.Transform = GetCupholderTransform()
            .WithRotation(Rotation.Identity);
        entity.SetParent(Chair, CupholderBoneName);
    }

    /// <summary>
    /// Launch <c>HeldEntity</c> from the cupholder.
    /// </summary>
    /// <param name="launchDirection">The worldspace direction in which <c>HeldEntity</c> shall
    /// be launched.</param>
    /// <param name="launchSpeed">The magnitude of the impulse that shall be applied to
    /// the <c>HeldEntity</c> when it is launched.</param>
    /// <param name="disableCollisionTime">The amount of time in seconds after launching <c>HeldEntity</c>
    /// for which <c>HeldEntity</c> shall have solid collisions disabled.</param>
    public async void LaunchHeldEntity(
        Vector3 launchDirection, 
        float launchSpeed, 
        float disableCollisionTime = 0f
        )
    {
        if (!HeldEntity.IsValid())
        {
            return;
        }
        // Make sure the held entity doesn't collide with the arm rest for now.
        HeldEntity.EnableSolidCollisions = false;
        HeldEntity.SetParent(null);
        HeldEntity.ApplyAbsoluteImpulse(launchDirection * launchSpeed);
        HeldEntity.ApplyLocalAngularImpulse(Vector3.Random * 1000f);
        if (disableCollisionTime > Time.Delta)
        {
            // Give the held entity some time to fly away from the chair.
            await GameTask.Delay(MathX.FloorToInt(disableCollisionTime * 1000f));
        }
        HeldEntity.EnableSolidCollisions = true;
        HeldEntity = null;
    }
}
