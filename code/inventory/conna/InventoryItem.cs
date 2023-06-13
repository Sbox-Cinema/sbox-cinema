using Sandbox;
using System.IO;
using System.Collections.Generic;
using System;
using Sandbox.UI;

namespace Conna.Inventory;

/// <summary>
/// An inventory item instance.
/// </summary>
public class InventoryItem : IValid
{
    /// <summary>
    /// The <see cref="InventoryContainer"/> that holds this item.
    /// </summary>
    public InventoryContainer Parent { get; set; }

    /// <summary>
    /// If this item is dropped this will be the <see cref="ItemEntity"/> that holds it.
    /// </summary>
    public ItemEntity WorldEntity { get; private set; }

    /// <summary>
    /// Is this item currently dropped (does it have a world entity?)
    /// </summary>
    public bool IsWorldEntity { get; private set; }

    /// <summary>
    /// When the item is created this will be its default stack size.
    /// </summary>
    public virtual ushort DefaultStackSize => 1;

    /// <summary>
    /// The maximum amount of this item that can be stacked.
    /// </summary>
    public virtual ushort MaxStackSize => 1;

    /// <summary>
    /// The world model to use when this item is dropped.
    /// </summary>
    public virtual string WorldModel => "models/sbox_props/burger_box/burger_box.vmdl";

    /// <summary>
    /// The description of this item.
    /// </summary>
    public virtual string Description => string.Empty;

    /// <summary>
    /// Does this item drop when a player dies?
    /// </summary>
    public virtual bool DropOnDeath => false;

    /// <summary>
    /// The color of this item (for display purposes.)
    /// </summary>
    public virtual Color Color => Color.White;

    /// <summary>
    /// The name of this item.
    /// </summary>
    public virtual string Name => string.Empty;

    /// <summary>
    /// A tint color to use for this item's icon.
    /// </summary>
    public virtual Color IconTintColor => Color.White;

    /// <summary>
    /// A display string for this item's primary use.
    /// </summary>
    public virtual string PrimaryUseHint => string.Empty;

    /// <summary>
    /// A display string for this item's secondary use.
    /// </summary>
    public virtual string SecondaryUseHint => string.Empty;

    /// <summary>
    /// The unique id of this item.
    /// </summary>
    public virtual string UniqueId => string.Empty;

    /// <summary>
    /// The icon to use for this item (for display purposes.)
    /// </summary>
    public virtual string Icon => string.Empty;

    public virtual IReadOnlySet<string> Tags => InternalTags;

    /// <summary>
    /// If this item is craftable then what items does it require and how many of them?
    /// </summary>
    public virtual Dictionary<string, int> RequiredItems => null;

    /// <summary>
    /// Is this item craftable?
    /// </summary>
    public virtual bool IsCraftable => false;

    protected HashSet<string> InternalTags = new(StringComparer.OrdinalIgnoreCase);

    public InventoryItem()
    {
        BuildTags(InternalTags);
    }

    /// <summary>
    /// Deserialize an item from a byte array.
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    public static InventoryItem Deserialize(byte[] data)
    {
        using (var stream = new MemoryStream(data))
        {
            using (var reader = new BinaryReader(stream))
            {
                return reader.ReadInventoryItem();
            }
        }
    }

    /// <summary>
    /// Serialize the item into a byte array.
    /// </summary>
    /// <returns></returns>
    public byte[] Serialize()
    {
        using (var stream = new MemoryStream())
        {
            using (var writer = new BinaryWriter(stream))
            {
                writer.Write(this);
                return stream.ToArray();
            }
        }
    }

    private ushort InternalStackSize;
    private bool InternalIsDirty;

    /// <summary>
    /// What is the current stack size of the item?
    /// </summary>
    public ushort StackSize
    {
        get => InternalStackSize;

        set
        {
            if (InternalStackSize != value)
            {
                InternalStackSize = value;
                IsDirty = true;

                if (InternalStackSize <= 0)
                    Remove();
            }
        }
    }

    /// <summary>
    /// Is this item an instance or a definition?
    /// </summary>
    public bool IsInstance => ItemId > 0;

    /// <summary>
    /// Has item data changed for this item since it was last networked?
    /// </summary>
    public bool IsDirty
    {
        get => InternalIsDirty;

        set
        {
            if (Game.IsServer)
            {
                if (Parent == null)
                {
                    InternalIsDirty = false;
                    return;
                }

                InternalIsDirty = value;

                if (InternalIsDirty)
                {
                    Parent.IsDirty = true;
                }
            }
        }
    }

    /// <summary>
    /// Is this item valid?
    /// </summary>
    public bool IsValid { get; set; }

    /// <summary>
    /// The item id of this item.
    /// </summary>
    public ulong ItemId { get; private set; }

    /// <summary>
    /// The slot this item currently sits in within its <see cref="InventoryContainer"/>.
    /// </summary>
    public ushort SlotId { get; set; }

    /// <summary>
    /// Set the world entity of this item.
    /// </summary>
    /// <param name="entity"></param>
    public void SetWorldEntity(ItemEntity entity)
    {
        WorldEntity = entity;
        IsWorldEntity = entity.IsValid();
        IsDirty = true;
        Remove();
    }

    /// <summary>
    /// Set (override) the item id of this item.
    /// </summary>
    /// <param name="itemId"></param>
    public void SetItemId(ulong itemId)
    {
        ItemId = itemId;
    }

    /// <summary>
    /// Clear this item's world entity.
    /// </summary>
    public void ClearWorldEntity()
    {
        WorldEntity = null;
        IsWorldEntity = false;
        IsDirty = true;
    }

    /// <summary>
    /// Remove this item from its parent <see cref="InventoryContainer"/>.
    /// </summary>
    public void Remove()
    {
        if (Parent.IsValid())
        {
            Parent.Remove(this);
        }
    }

    /// <summary>
    /// Replace this item in its parent <see cref="InventoryContainer"/> with another item.
    /// </summary>
    /// <param name="other"></param>
    public void Replace(InventoryItem other)
    {
        if (Parent.IsValid())
        {
            Parent.Replace(SlotId, other);
        }
    }

    public virtual bool OnTrySwap(InventoryItem other)
    {
        return true;
    }

    /// <summary>
    /// Is this item the same type as another item?
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool IsSameType(InventoryItem other)
    {
        return (GetType() == other.GetType() && UniqueId == other.UniqueId);
    }

    /// <summary>
    /// Should this item stack with another item?
    /// </summary>
    /// <param name="other"></param>
    /// <returns></returns>
    public virtual bool CanStackWith(InventoryItem other)
    {
        return true;
    }

    public virtual void AddTooltipInfo(Panel container)
    {

    }

    /// <summary>
    /// Serialize this item ready for sending over the network.
    /// </summary>
    /// <param name="writer"></param>
    public virtual void Write(BinaryWriter writer)
    {
        if (WorldEntity.IsValid())
        {
            writer.Write(true);
            writer.Write(WorldEntity.NetworkIdent);
        }
        else
        {
            writer.Write(false);
        }

    }

    /// <summary>
    /// Deserialize this item from data sent over the network.
    /// </summary>
    /// <param name="reader"></param>
    public virtual void Read(BinaryReader reader)
    {
        IsWorldEntity = reader.ReadBoolean();

        if (IsWorldEntity)
        {
            WorldEntity = (Entity.FindByIndex(reader.ReadInt32()) as ItemEntity);
            return;
        }

        if (WorldEntity.IsValid())
        {
            if (Game.IsServer)
            {
                WorldEntity.Delete();
            }

            WorldEntity = null;
        }
    }

    /// <summary>
    /// Called when the item is removed.
    /// </summary>
    public virtual void OnRemoved()
    {

    }

    /// <summary>
    /// Called when the item is created.
    /// </summary>
    public virtual void OnCreated()
    {

    }

    /// <summary>
    /// Build a list of tags that describe this item.
    /// </summary>
    /// <param name="tags"></param>
    protected virtual void BuildTags(HashSet<string> tags)
    {

    }

    public override int GetHashCode()
    {
        return HashCode.Combine(IsValid, UniqueId, ItemId, StackSize);
    }
}
