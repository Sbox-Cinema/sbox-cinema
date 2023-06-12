namespace Conna.Inventory;

/// <summary>
/// An interface to describe a resource item.
/// </summary>
public interface IResourceItem
{
	/// <summary>
	/// The item resource asset data.
	/// </summary>
	public ItemResource Resource { get; }

	/// <summary>
	/// Load resource data from an asset.
	/// </summary>
	/// <param name="resource"></param>
	public void LoadResource( ItemResource resource );
}
