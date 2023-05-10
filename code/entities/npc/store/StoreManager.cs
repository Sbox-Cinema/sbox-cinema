using System.Collections.Generic;

namespace Cinema;

public static partial class StoreManager
{
    private static Dictionary<ulong, Store> Stores { get; set; } = new();

    private static ulong NextStoreId { get; set; } = 1;

    public static ulong RegisterStore(Store store)
    {
        var id = NextStoreId++;
        Stores[id] = store;

        return id;
    }

    public static Store GetStore(ulong id)
    {
        return Stores[id];
    }
}
