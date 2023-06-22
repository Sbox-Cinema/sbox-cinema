using Conna.Inventory;
using Sandbox;

namespace Cinema;

[ItemClass(typeof(HandheldItem))]
[GameResource("Handheld Item", "handheld", "Handheld Item in Cinema", Icon = "lunch_dining")]
public class HandheldResource : ItemResource
{
    [Property]
    public string ClassName { get; set; }

    [Property, ResourceType("vmdl")]
    public string ViewModel { get; set; } = "models/dev/error.vmdl_c";
}
