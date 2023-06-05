using System;
using System.Collections.Generic;
using System.Linq;
using Sandbox;

namespace Cinema.UI;

public partial class StoreInterface : IMenuScreen
{
    public static StoreInterface Instance { get; set; }
    public bool IsOpen { get; protected set; }
    public string Name => $"Store: {Store.Name}";

    public Store Store
    {
        get { return _Store; }
        set
        {
            _Store = value;
            if (_Store is null)
            {
                HoveredItem = null;
                return;
            }

            HoveredItem = Items.FirstOrDefault();
        }
    }
    private Store _Store;

    public List<StoreItem> Items => Store?.ItemsForSale ?? new();

    public StoreItem HoveredItem { get; set; }

    public StoreInterface()
    {
        Instance = this;
    }

    public bool Open()
    {
        IsOpen = true;
        return true;
    }

    public void Close()
    {
        Store = null;
        IsOpen = false;
    }

    public void OnItemClicked(StoreItem item)
    {
        if (Game.LocalPawn is not Player player) return;

        Store?.Purchase(player, item);
    }

    public void OnItemHovered(StoreItem item)
    {
        HoveredItem = item;
    }

    public override void Tick()
    {
        base.Tick();
    }

    protected override int BuildHash()
    {
        var hash = 11;

        foreach (var item in Items)
        {
            hash = hash * 31 + item.GetHashCode();
        }

        return HashCode.Combine(IsOpen, hash, Store?.StoreId);
    }
}
