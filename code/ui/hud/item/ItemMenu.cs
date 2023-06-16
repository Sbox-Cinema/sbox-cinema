using Conna.RadialMenu;
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace Cinema.UI;

[StyleSheet("/ui/hud/item/ItemMenu.scss")]
public partial class ItemMenu : RadialMenu
{

    public static List<WeaponBase> Weapons => (Game.LocalPawn as Player)?.Weapons;

    public override string Button => "hotwheel";

    public override void Populate()
    {
        foreach (var wep in Weapons)
        {
            AddItem(wep.Name, wep.Description, wep.Icon, () => SelectItem(wep));
        }
    }

    protected override bool ShouldOpen()
    {
        var player = Game.LocalPawn as Player;
        return player.IsValid() && !player.IsMenuOpen;
    }

    private static void SelectItem(WeaponBase selected)
    {
        if (Game.LocalPawn is not Player player) return;

        player.ChangeActiveChild(selected);
    }
}
