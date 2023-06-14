using System.Linq;
using System.Collections.Generic;
using System.IO;
using Conna.Inventory;
using Sandbox;

namespace Cinema;


public partial class TrashBag : InventoryItem
{
    public override string Name => "Trash Bag";
    public override string Description => "Used to pickup trash";
    public override string Icon => "";
    public override string UniqueId => "trash_bag";

    public List<string> Contents { get; private set; } = new List<string>();

    public int Capacity { get; private set; } = 30;
    public int RemainingSpace => Capacity - Contents.Count;

    /// <summary>
    /// Adds an item of trash to the trash bag.
    /// </summary>
    /// <param name="item">Name of the trash item</param>
    public bool Add(string item)
    {
        if (Game.IsClient) throw new System.Exception("Cannot add to trashbag on client");
        if (Contents.Count >= Capacity) return false;
        Log.Info("adding trash to bag");
        Contents.Add(item);
        IsDirty = true;
        return true;
    }

    /// <summary>
    /// Removes the last trash from the bag and returns it.
    /// </summary>
    /// <returns>Name of trash</returns>
    public string RemoveLast()
    {
        if (Game.IsClient) throw new System.Exception("Cannot remove trash on client");
        if (Contents.Count == 0) return null;
        var topTrash = Contents.LastOrDefault();
        Contents.RemoveAt(Contents.Count - 1);
        IsDirty = true;
        return topTrash;
    }

    /// <summary>
    /// Removes all the trash from the bag.
    /// </summary>
    public void RemoveAll()
    {
        if (Game.IsClient) throw new System.Exception("Cannot remove trash on client");
        Contents.Clear();
        IsDirty = true;
    }

    /// <summary>
    /// Changes the capacity of the trash bag. Any items that exceed the new capacity will be removed.
    /// </summary>
    /// <param name="newCapacity"></param>
    public void SetCapacity(int newCapacity)
    {
        if (Game.IsClient) throw new System.Exception("Cannot change capacity on client");
        if (newCapacity < 0) return;
        if (newCapacity < Contents.Count)
        {
            Contents.RemoveRange(newCapacity, Contents.Count - newCapacity);
        }
        Capacity = newCapacity;
        IsDirty = true;
    }

    public override void Write(BinaryWriter writer)
    {
        base.Write(writer);
        writer.Write(Contents.Count);
        foreach (var item in Contents)
        {
            writer.Write(item);
        }
        writer.Write(Capacity);
    }

    public override void Read(BinaryReader reader)
    {
        base.Read(reader);
        var count = reader.ReadInt32();
        Contents.Clear();
        for (int i = 0; i < count; i++)
        {
            Contents.Add(reader.ReadString());
        }
        Capacity = reader.ReadInt32();
    }

    [ConCmd.Client("trash.debug")]
    protected static void DebugContents()
    {
        if (ConsoleSystem.Caller.Pawn is not Player player) return;
        var trashBag = player.Inventory.FindItems<TrashBag>().FirstOrDefault();
        if (trashBag == null) return;
        Log.Info($"Trash bag contents ({trashBag.Contents.Count}/{trashBag.Capacity}): {string.Join(", ", trashBag.Contents)}");
    }

}
