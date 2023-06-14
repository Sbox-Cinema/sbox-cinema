using Sandbox;
using System.Collections.Generic;

namespace Conna.Inventory;

public class ItemResource : GameResource
{
	/// <summary>
	/// The item name.
	/// </summary>
	[Property]
	public string ItemName { get; set; }

	/// <summary>
	/// The unique id of the item.
	/// </summary>
	[Property]
	public string UniqueId { get; set; }

	/// <summary>
	/// The item description.
	/// </summary>
	[Property]
	public string Description { get; set; }

	/// <summary>
	/// The icon to use for the item.
	/// </summary>
	[Property, ResourceType( "png" )]
	public string Icon { get; set; }

	/// <summary>
	/// The world model to use when the item is dropped.
	/// </summary>
	[Property, ResourceType( "vmdl" )]
	public string WorldModel { get; set; } = "models/sbox_props/burger_box/burger_box.vmdl";

	/// <summary>
	/// A list of tags that describe the item.
	/// </summary>
	[Property]
	public List<string> Tags { get; set; } = new();

	protected override void PostLoad()
	{
		if ( Game.IsServer || Game.IsClient )
		{
			InventorySystem.ReloadDefinitions();
		}

		base.PostLoad();
	}

	protected override void PostReload()
	{
		if ( Game.IsServer || Game.IsClient )
		{
			InventorySystem.ReloadDefinitions();
		}
		
		base.PostReload();
	}
}
