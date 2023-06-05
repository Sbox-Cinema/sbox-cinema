using Sandbox;
using System.Collections.Generic;

namespace Cinema;

public partial class Store : BaseNetworkable, INetworkSerializer
{
    public string Name { get; set; }
    public ulong StoreId { get; set; }

    public List<StoreItem> ItemsForSale { get; set; } = new();

    public Store()
    {
        if (Game.IsServer)
        {
            StoreId = StoreManager.RegisterStore(this);
        }
    }

    /// <summary>
    /// Have a player purchase an item, can only be called serverside
    /// </summary>
    /// <param name="player">The player who is purchasing</param>
    /// <param name="item">The item to purchase</param>
    public void Purchase(Player player, StoreItem item)
    {
        if (!ItemsForSale.Contains(item)) return;
        if (player.Money < item.Cost) return;

        if (Game.IsClient)
        {
            ClientPurchase(StoreId, player.NetworkIdent, ItemsForSale.IndexOf(item));
            return;
        }

        player.TakeMoney(item.Cost);
        item.OnPurchase?.Invoke(player);
    }

    [ConCmd.Server]
    private static void ClientPurchase(ulong storeId, int playerId, int itemIndex)
    {
        var store = StoreManager.GetStore(storeId);
        var player = Entity.FindByIndex(playerId) as Player;
        if (!player.IsValid()) return;

        store.Purchase(player, store.ItemsForSale[itemIndex]);
    }

    public void Write(NetWrite write)
    {
        write.Write(Name);
        write.Write(StoreId);
        write.Write(ItemsForSale.Count);
        foreach (var item in ItemsForSale)
        {
            write.Write(item.Name);
            write.Write(item.Description);
            write.Write(item.Cost);
            write.Write(item.Icon);
        }
    }

    public void Read(ref NetRead read)
    {
        Name = read.ReadString();
        StoreId = read.Read<ulong>();
        ItemsForSale = new List<StoreItem>();
        var count = read.Read<int>();
        for (int i = 0; i < count; i++)
        {
            var name = read.ReadString();
            var description = read.ReadString();
            var cost = read.Read<int>();
            var icon = read.ReadString();
            var item = new StoreItem
            {
                Name = name,
                Description = description,
                Cost = cost,
                Icon = icon
            };
            ItemsForSale.Add(item);
        }
    }
}
