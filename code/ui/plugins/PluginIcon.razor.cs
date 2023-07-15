using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class PluginIcon : Panel
{
    public Package Package { get; set; }
    public Action OnClicked { get; set; }
}
