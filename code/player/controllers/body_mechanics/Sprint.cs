using Sandbox;

namespace Cinema;

public partial class SprintMechanic : PlayerBodyControllerMechanic
{
    /// <summary>
    /// Sprint has a higher priority than other mechanics.
    /// </summary>
    public override int SortOrder => 10;
    public override float? WishSpeed => 320f;

    protected override bool ShouldStart()
    {
        if (!Input.Down(InputButton.Run))
            return false;
        if (Player.MoveInput.Length == 0)
            return false;

        return true;
    }
}
