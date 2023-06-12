namespace Conna.Inventory;

/// <summary>
/// Implement this interface on an item if the item can contain other items.
/// </summary>
public interface IContainerItem
{
	/// <summary>
	/// The <see cref="InventoryContainer"/> this item holds.
	/// </summary>
	public InventoryContainer Container { get; }

	/// <summary>
	/// The name of the container (for display purposes.)
	/// </summary>
	public string ContainerName { get; }
}
