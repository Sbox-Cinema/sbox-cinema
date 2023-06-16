using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System.Text;

namespace Cinema.UI;

public partial class Crosshair : Panel
{
    public Crosshair()
    {
        StyleSheet.Load("/ui/hud/crosshair/Crosshair.scss");
        Log.Info("Workin");
    }
}
