namespace Cinema.UI;

using Sandbox;
using Sandbox.UI;


public partial class Hud : RootPanel
{
    public static Hud Instance { get; private set; }
    public Hud()
    {
        Instance = this;
    }

    public bool ShouldHide { get; set; }
    public string GetVisibleClass() => ShouldHide ? "" : "visible";
}
