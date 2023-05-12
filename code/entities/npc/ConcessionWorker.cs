using Sandbox;
using System.Collections.Generic;

namespace Cinema;

public partial class ConcessionWorker : NpcBase
{
    public override string Name => "Concession Employee";
    public override string Description => "Buy some snacks!";
    public override bool ToggleUse => true;

    [Net]
    public Store Store { get; set; }

    public override void Spawn()
    {
        base.Spawn();

        Store = new Store()
        {
            ItemsForSale = new List<StoreItem> {
                StoreItem.Weapon("Hotdog", "99% Beef", "ui/icons/hotdog.png", 10, "Hotdog"),
                StoreItem.Weapon("Nachos", "Not your cheese!", "ui/icons/nachos.png", 20, "Nachos"),
                StoreItem.Weapon("Popcorn", "Pop Pop Pop", "ui/icons/popcorn.png", 30, "Popcorn"),
                StoreItem.Weapon("Soda", "America Juice", "ui/icons/soda.png", 5, "Soda")
            }
        };
    }

    public override bool IsUsable(Entity user)
    {
        if (user is not Player player) return false;

        var canPurchaseConcessions = player.Job.HasAbility(Jobs.JobAbilities.PurchaseConcessions);

        return canPurchaseConcessions;
    }

    public override void OnClientUse(Player player)
    {
        UI.StoreInterface.Instance.OpenForStore(Store);
    }

    public override void OnClientStopUse(Player player)
    {
        UI.StoreInterface.Instance.Close();
    }

}
