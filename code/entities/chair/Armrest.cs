using Sandbox;

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
    public ModelEntity Cupholded { get; set; }
    [Net]
    public States State { get; set; } = States.Lowered;
    [Net]
    public Sides Side { get; set; }

    public string CupholderBoneName => Side switch
    {
        Sides.Left  => "cupholder_l",
        _           => "cupholder_r"
    };

    public string ArmrestToggleParameter => Side switch
    {
        Sides.Left => "toggle_left_armrest",
        _ => "toggle_right_armrest"
    };

    public void HandleAnimTag(string tag)
    {
        if (tag.Contains("launch"))
        {
            Launch();
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

    public void CupholdEntity(ModelEntity entity)
    {
        if (State != States.Lowered)
        {
            return;
        }
        Cupholded = entity;
        entity.Transform = GetCupholderTransform()
            .WithRotation(Rotation.Identity);
        entity.SetParent(Chair, CupholderBoneName);
    }

    private Transform GetCupholderTransform()
        => Chair.GetBoneTransform(CupholderBoneName);

    public async void Launch()
    {
        if (!Cupholded.IsValid())
        {
            Log.Trace($"{Chair.Name} - No entity in {Side} cupholder.");
            return;
        }
        // Make sure the cup doesn't collide with the arm rest for now.
        Cupholded.EnableSolidCollisions = false;
        Cupholded.SetParent(null);
        var launchAngle = (Chair.Rotation.Backward + Chair.Rotation.Up).Normal;
        Cupholded.ApplyAbsoluteImpulse(launchAngle * 250f);
        Cupholded.ApplyLocalAngularImpulse(Vector3.Random * 1000f);
        // Give the cup some time to fly away from the chair.
        await GameTask.Delay(200);
        Cupholded.EnableSolidCollisions = true;
        Cupholded = null;
    }
}
