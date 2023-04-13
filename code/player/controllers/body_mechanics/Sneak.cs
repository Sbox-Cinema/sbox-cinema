using Sandbox;

namespace Cinema;

public partial class SneakMechanic : PlayerBodyControllerMechanic
{
    public override int SortOrder => 9;
    public override float? WishSpeed => 100f;

    protected override bool ShouldStart()
    {
        if (!Input.Down(InputButton.Walk))
            return false;
        if (Player.MoveInput.Length == 0)
            return false;

        return true;
    }
}
