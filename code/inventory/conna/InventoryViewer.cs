using Sandbox;
using System.Collections.Generic;

namespace Conna.Inventory;

/// <summary>
/// This component should be added to an <see cref="IClient"/>. This allows inventories be networked to them.
/// </summary>
public partial class InventoryViewer : EntityComponent, IValid
{
	/// <summary>
	/// Whether or not the inventory viewer is valid.
	/// </summary>
	public bool IsValid => Entity.IsValid();

	/// <summary>
	/// All container ids that this inventory viewer can see.
	/// </summary>
	[Net] public IList<ulong> ContainerIds { get; private set; } = new List<ulong>();

	/// <summary>
	/// The container that this viewer is currently viewing.
	/// </summary>
	public IEnumerable<InventoryContainer> Containers
	{
		get
		{
			foreach ( var id in ContainerIds )
			{
				yield return InventorySystem.Find( id );
			}
		}
	}

	/// <summary>
	/// Set the container this viewer is currently viewing.
	/// </summary>
	public void AddContainer( InventoryContainer container )
	{
		if ( !ContainerIds.Contains( container.ContainerId ) )
		{
			ContainerIds.Add( container.ContainerId );
		}
	}

	/// <summary>
	/// Clear the container this viewer is currently viewing.
	/// </summary>
	public void ClearContainers()
	{
		ContainerIds.Clear();
	}
}
