using Sandbox;
using System.Collections.Generic;
using System.IO;

namespace Conna.Inventory;

public interface IInventoryItem : IValid
{
	/// <summary>
	/// Is this item valid?
	/// </summary>
	public new bool IsValid { get; set; }

	/// <summary>
	/// The <see cref="InventoryContainer"/> that holds this item.
	/// </summary>
	public InventoryContainer Parent { get; set; }

	/// <summary>
	/// If this item is dropped this will be the <see cref="ItemEntity"/> that holds it.
	/// </summary>
	public ItemEntity WorldEntity { get; }

	/// <summary>
	/// Is this item currently dropped (does it have a world entity?)
	/// </summary>
	public bool IsWorldEntity { get; }

	/// <summary>
	/// When the item is created this will be its default stack size.
	/// </summary>
	public ushort DefaultStackSize { get; }

	/// <summary>
	/// The maximum amount of this item that can be stacked.
	/// </summary>
	public ushort MaxStackSize { get; }

	/// <summary>
	/// The world model to use when this item is dropped.
	/// </summary>
	public string WorldModel { get; }

	/// <summary>
	/// The unique id of this item.
	/// </summary>
	public string UniqueId { get; }

	/// <summary>
	/// The description of this item.
	/// </summary>
	public string Description { get; }

	/// <summary>
	/// The name of this item.
	/// </summary>
	public string Name { get; }

	/// <summary>
	/// Tags associated with the item.
	/// </summary>
	public IReadOnlySet<string> Tags { get; }

	/// <summary>
	/// What is the current stack size of the item?
	/// </summary>
	public ushort StackSize { get; set; }

	/// <summary>
	/// Has item data changed for this item since it was last networked?
	/// </summary>
	public bool IsDirty { get; set; }

	/// <summary>
	/// The item id of this item.
	/// </summary>
	public ulong ItemId { get; }

	/// <summary>
	/// The slot this item currently sits in within its <see cref="InventoryContainer"/>.
	/// </summary>
	public ushort SlotId { get; set; }

	/// <summary>
	/// Is this item the same type as another item?
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool IsSameType(IInventoryItem other);

	/// <summary>
	/// Should this item stack with another item?
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool CanStackWith(IInventoryItem other);

	/// <summary>
	/// Can this item be swapped with another item?
	/// </summary>
	/// <param name="other"></param>
	/// <returns></returns>
	public bool OnTrySwap(IInventoryItem other);

	/// <summary>
	/// Set (override) the item id of this item.
	/// </summary>
	/// <param name="itemId"></param>
	public void SetItemId(ulong itemId);

	/// <summary>
	/// Set the world entity of this item.
	/// </summary>
	/// <param name="entity"></param>
	public void SetWorldEntity(ItemEntity entity);

	/// <summary>
	/// Clear this item's world entity.
	/// </summary>
	public void ClearWorldEntity();

	/// <summary>
	/// Serialize the item into a byte array.
	/// </summary>
	/// <returns></returns>
	public byte[] Serialize();

	/// <summary>
	/// Called when the item is created.
	/// </summary>
	public void OnCreated();

	/// <summary>
	/// Remove this item from its parent <see cref="InventoryContainer"/>.
	/// </summary>
	public void Remove();

	/// <summary>
	/// Called when the item is removed.
	/// </summary>
	public void OnRemoved();

	/// <summary>
	/// Serialize this item ready for sending over the network.
	/// </summary>
	/// <param name="writer"></param>
	public void Write(BinaryWriter writer);

	/// <summary>
	/// Deserialize this item from data sent over the network.
	/// </summary>
	/// <param name="reader"></param>
	public void Read(BinaryReader reader);
}
