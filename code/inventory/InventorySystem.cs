using Sandbox;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Conna.Inventory;

public static partial class InventorySystem
{
	public enum NetworkEvent
	{
		DirtyItems,
		Move,
		Split,
		Transfer,
		Give,
		Take
	}

	private static Dictionary<ulong, InventoryContainer> Containers { get; set; } = new();
	private static Dictionary<ulong, InventoryItem> Items { get; set; } = new();
	private static Dictionary<string, InventoryItem> Definitions { get; set; } = new();
	private static List<InventoryContainer> DirtyList { get; set; } = new();
	private static Queue<ulong> OrphanedItems { get; set; } = new();
	private static TypeDescription ItemEntityType { get; set; }

	private static ulong NextContainerId { get; set; }
	private static ulong NextItemId { get; set; }

	public static ItemEntity CreateItemEntity( InventoryItem item = null )
	{
		var type = GetItemEntityType();
		var entity = type.Create<ItemEntity>();

		if ( item.IsValid() )
		{
			entity.SetItem( item );
		}

		return entity;
	}

	public static TypeDescription GetItemEntityType()
	{
		if ( ItemEntityType == null )
		{
			var type = TypeLibrary.GetTypesWithAttribute<ItemEntityAttribute>().FirstOrDefault();

			if ( type.Type.TargetType?.IsAssignableTo( typeof( ItemEntity ) ) ?? false )
				ItemEntityType = type.Type;
			else
				ItemEntityType = TypeLibrary.GetType<ItemEntity>();
		}

		return ItemEntityType;
	}

	public static void Initialize()
	{
		ReloadDefinitions();
	}

	public static void AddToDirtyList( InventoryContainer container )
	{
		DirtyList.Add( container );
	}

	public static void Serialize( BinaryWriter writer )
	{
		writer.Write( NextContainerId );
		writer.Write( NextItemId );
	}

	public static void Deserialize( BinaryReader reader )
	{
		NextContainerId = reader.ReadUInt64();
		NextItemId = reader.ReadUInt64();
		ReassignIds();
	}

	public static ulong GenerateContainerId()
	{
		return ++NextContainerId;
	}

	public static ulong GenerateItemId()
	{
		return ++NextItemId;
	}

	public static void ReassignId( InventoryContainer container, ulong containerId )
	{
		Containers.Remove( container.ContainerId );
		container.SetContainerId( containerId );
		Containers.Add( containerId, container );
	}

	public static void ReassignIds()
	{
		var items = Items.Values
			.Where( i => i.Parent.IsValid() && i.Parent.Entity.IsValid() )
			.Where( i => i.Parent.Entity.IsFromMap )
			.ToList();

		foreach ( var item in items )
		{
			Items.Remove( item.ItemId );
		}

		foreach ( var item in items )
		{
			item.SetItemId( GenerateItemId() );
			Items.Add( item.ItemId, item );
		}

		var containers = Containers.Values
			.Where( c => c.Entity.IsValid() && c.Entity.IsFromMap )
			.ToList();

		foreach ( var container in containers )
		{
			Containers.Remove( container.ContainerId );
		}

		foreach ( var container in containers )
		{
			container.SetContainerId( GenerateContainerId() );
			Containers.Add( container.ContainerId, container );
		}
	}

	public static IEnumerable<InventoryItem> GetDefinitions()
	{
		foreach ( var kv in Definitions )
		{
			yield return kv.Value;
		}
	}

	public static ulong Register( InventoryContainer container, ulong containerId = 0 )
	{
		if ( containerId == 0 && Game.IsClient )
		{
			throw new Exception( "Cannot register a new InventoryContainer client-side without an explicit inventory id!" );
		}

		if ( containerId == 0 )
		{
			containerId = GenerateContainerId();
		}

		container.SetContainerId( containerId );
		Containers[containerId] = container;

		return containerId;
	}

	public static List<InventoryItem> Remove( InventoryContainer container, bool destroyItems = true )
	{
		var containerId = container.ContainerId;

		if ( Containers.Remove( containerId ) )
		{
			var itemList = container.RemoveAll();

			if ( destroyItems )
			{
				for ( var i = itemList.Count - 1; i >= 0; i-- )
				{
					RemoveItem( itemList[i] );
				}
			}

			container.SetContainerId( 0 );

			return itemList;
		}

		return null;
	}

	public static InventoryContainer Find( ulong inventoryId )
	{
		if ( Containers.TryGetValue( inventoryId, out var container ) )
		{
			return container;
		}

		return null;
	}

	public static InventoryItem FindInstance( ulong itemId )
	{
		Items.TryGetValue( itemId, out var instance );
		return instance;
	}

	public static T FindInstance<T>( ulong itemId ) where T : InventoryItem
	{
		return (FindInstance( itemId ) as T);
	}

	public static void RemoveItem( InventoryItem instance )
	{
		var itemId = instance.ItemId;

		if ( Items.Remove( itemId ) )
		{
			instance.Parent?.Remove( itemId );
			instance.OnRemoved();
		}
	}

	public static InventoryItem DuplicateItem( InventoryItem item )
	{
		var duplicate = CreateItem( item.UniqueId );
		
		using ( var writeStream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( writeStream ) )
			{
				item.Write( writer );
			}

			var data = writeStream.ToArray();

			using ( var readStream = new MemoryStream( data ) )
			{
				using ( var reader = new BinaryReader( readStream ) )
				{
					duplicate.Read( reader );
				}
			}
		}

		return duplicate;
	}

	public static T CreateItem<T>( ulong itemId = 0 ) where T : InventoryItem
	{
		if ( itemId > 0 && Items.TryGetValue( itemId, out var instance ) )
		{
			return instance as T;
		}

		if ( itemId == 0 )
		{
			itemId = GenerateItemId();
		}

		instance = TypeLibrary.Create<InventoryItem>( typeof( T ) );
		instance.SetItemId( itemId );
		instance.IsValid = true;
		instance.StackSize = instance.DefaultStackSize;
		instance.OnCreated();

		Items[itemId] = instance;

		return instance as T;
	}

	public static InventoryItem GetDefinition( string uniqueId )
	{
		if ( string.IsNullOrEmpty( uniqueId ) )
			return null;

		if ( Definitions.TryGetValue( uniqueId, out var definition ) )
		{
			return definition;
		}

		return null;
	}

	public static InventoryItem CreateItem( Type type, ulong itemId = 0 )
	{
		if ( itemId > 0 && Items.TryGetValue( itemId, out var instance ) )
		{
			return instance;
		}

		if ( itemId == 0 )
		{
			itemId = GenerateItemId();
		}

		instance = TypeLibrary.Create<InventoryItem>( type );
		instance.SetItemId( itemId );
		instance.IsValid = true;
		instance.StackSize = instance.DefaultStackSize;
		instance.OnCreated();

		Items[itemId] = instance;

		return instance;
	}

	public static InventoryItem CreateItem( string uniqueId, ulong itemId = 0 )
	{
		if ( itemId > 0 && Items.TryGetValue( itemId, out var instance ) )
		{
			return instance;
		}

		if ( itemId == 0 )
		{
			itemId = GenerateItemId();
		}

		if ( !Definitions.ContainsKey( uniqueId ) )
		{
			Log.Error( $"Unable to create an item with unknown unique id: {uniqueId}" );
			return null;
		}

		var definition = Definitions[uniqueId];

		instance = TypeLibrary.Create<InventoryItem>( definition.GetType() );
		instance.SetItemId( itemId );
		instance.IsValid = true;
		instance.StackSize = instance.DefaultStackSize;

		if ( instance is IResourceItem a && definition is IResourceItem b )
		{
			a.LoadResource( b.Resource );
		}

		instance.OnCreated();

		Items[itemId] = instance;

		return instance;
	}

	public static T CreateItem<T>( string uniqueId ) where T : InventoryItem
	{
		return (CreateItem( uniqueId ) as T);
	}

	public static void ClientJoined( IClient client )
	{
		client.Components.Create<InventoryViewer>();
	}

	public static void ClientDisconnected( IClient client )
	{
		foreach ( var container in Containers.Values )
		{
			if ( container.IsConnected( client ) )
			{
				container.RemoveConnection( client );
			}
		}
	}

	public static void SendTransferEvent( InventoryContainer from, InventoryContainer to, ushort fromSlot )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( fromSlot );
				writer.Write( from.ContainerId );
				writer.Write( to.ContainerId );
				SendEventDataToServer( NetworkEvent.Transfer, Convert.ToBase64String( stream.ToArray() ) );
			}
		}
	}

	public static void SendSplitEvent( InventoryContainer from, InventoryContainer to, ushort fromSlot, ushort toSlot )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( fromSlot );
				writer.Write( from.ContainerId );
				writer.Write( toSlot );
				writer.Write( to.ContainerId );
				SendEventDataToServer( NetworkEvent.Split, Convert.ToBase64String( stream.ToArray() ) );
			}
		}
	}

	public static void SendMoveEvent( InventoryContainer from, InventoryContainer to, ushort fromSlot, ushort toSlot )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( fromSlot );
				writer.Write( from.ContainerId );
				writer.Write( toSlot );
				writer.Write( to.ContainerId );
				SendEventDataToServer( NetworkEvent.Move, Convert.ToBase64String( stream.ToArray() ) );
			}
		}
	}

	public static void SendGiveItemEvent( To to, InventoryContainer container, ushort slotId, InventoryItem instance )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( container.ContainerId );
				writer.Write( instance );
				writer.Write( slotId );
				SendEventDataToClient( to, NetworkEvent.Give, stream.ToArray() );
			}
		}
	}

	public static void SendTakeItemEvent( To to, InventoryContainer container, ushort slotId )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( container.ContainerId );
				writer.Write( slotId );
				SendEventDataToClient( to, NetworkEvent.Take, stream.ToArray() );
			}
		}
	}

	public static void SendDirtyItemsEvent( To to, InventoryContainer container )
	{
		using ( var stream = new MemoryStream() )
		{
			using ( var writer = new BinaryWriter( stream ) )
			{
				writer.Write( container.ContainerId );

				ushort dirtyCount = 0;
				var itemList = container.ItemList;

				for ( var i = 0; i < itemList.Count; i++ )
				{
					var item = itemList[i];

					if ( item != null && item.IsDirty )
					{
						dirtyCount++;
					}
				}

				writer.Write( dirtyCount );

				for ( var i = 0; i < itemList.Count; i++ )
				{
					var item = itemList[i];

					if ( item != null && item.IsDirty )
					{
						writer.Write( item );
						item.IsDirty = false;
					}
				}

				SendEventDataToClient( to, NetworkEvent.DirtyItems, stream.ToArray() );
			}
		}
	}

	private static void ProcessTakeItemEvent( BinaryReader reader )
	{
		var inventoryId = reader.ReadUInt64();
		var container = Find( inventoryId );
		container?.ProcessTakeItemEvent( reader );
	}

	private static void ProcessGiveItemEvent( BinaryReader reader )
	{
		var inventoryId = reader.ReadUInt64();
		var container = Find( inventoryId );
		container?.ProcessGiveItemEvent( reader );
	}

	private static void ProcessTransferInventoryEvent( BinaryReader reader )
	{
		var fromSlot = reader.ReadUInt16();
		var fromId = reader.ReadUInt64();
		var toId = reader.ReadUInt64();
		var fromInventory = Find( fromId );
		var toInventory = Find( toId );

		if ( fromInventory == null )
		{
			return;
		}

		if ( toInventory == null )
		{
			return;
		}

		if ( Game.IsServer )
		{
			var item = fromInventory.GetFromSlot( fromSlot );

			if ( item.IsValid() )
			{
				var remaining = toInventory.Stack( item );

				if ( remaining > 0 )
				{
					item.StackSize = remaining;
					return;
				}

				fromInventory.ClearSlot( fromSlot );
			}
		}
	}

	private static void ProcessSplitInventoryEvent( BinaryReader reader )
	{
		var fromSlot = reader.ReadUInt16();
		var fromId = reader.ReadUInt64();
		var toSlot = reader.ReadUInt16();
		var toId = reader.ReadUInt64();
		var fromInventory = Find( fromId );
		var toInventory = Find( toId );

		if ( fromInventory == null )
		{
			return;
		}

		if ( toInventory == null )
		{
			return;
		}

		if ( Game.IsServer )
		{
			fromInventory.Split( toInventory, fromSlot, toSlot );
		}
	}

	private static void ProcessMoveInventoryEvent( BinaryReader reader )
	{
		var fromSlot = reader.ReadUInt16();
		var fromId = reader.ReadUInt64();
		var toSlot = reader.ReadUInt16();
		var toId = reader.ReadUInt64();
		var fromInventory = Find( fromId );
		var toInventory = Find( toId );

		if ( fromInventory == null )
		{
			return;
		}

		if ( toInventory == null )
		{
			return;
		}

		if ( Game.IsServer )
		{
			fromInventory.Move( toInventory, fromSlot, toSlot );
		}
	}

	private static void ProcessSendDirtyItemsEvent( BinaryReader reader )
	{
		var container = Find( reader.ReadUInt64() );
		if ( container == null ) return;

		var itemCount = reader.ReadUInt16();

		for ( var i = 0; i < itemCount; i++ )
		{
			var item = reader.ReadInventoryItem();

			if ( item != null )
			{
				container.InvokeDataChanged( item.SlotId );
			}
		}
	}

	[ConCmd.Server]
	public static void SendEventDataToServer( NetworkEvent type, string data )
	{
		var decoded = Convert.FromBase64String( data );

		using ( var stream = new MemoryStream( decoded ) )
		{
			using ( var reader = new BinaryReader( stream ) )
			{
				switch ( type )
				{
					case NetworkEvent.Transfer:
						ProcessTransferInventoryEvent( reader );
						break;
					case NetworkEvent.Split:
						ProcessSplitInventoryEvent( reader );
						break;
					case NetworkEvent.Move:
						ProcessMoveInventoryEvent( reader );
						break;
				}
			}
		}
	}

	[ClientRpc]
	public static void SendEventDataToClient( NetworkEvent type, byte[] data )
	{
		using ( var stream = new MemoryStream( data ) )
		{
			using ( var reader = new BinaryReader( stream ) )
			{
				switch ( type )
				{
					case NetworkEvent.DirtyItems:
						ProcessSendDirtyItemsEvent( reader );
						break;
					case NetworkEvent.Move:
						ProcessMoveInventoryEvent( reader );
						break;
					case NetworkEvent.Give:
						ProcessGiveItemEvent( reader );
						break;
					case NetworkEvent.Take:
						ProcessTakeItemEvent( reader );
						break;
				}
			}
		}
	}

	public static void ReloadDefinitions()
	{
		Definitions.Clear();

		if ( ResourceLibrary == null )
		{
			Log.Info( "Unable to reload Forsaken item definitions because ResourceLibrary is null!" );
			return;
		}

		if ( TypeLibrary == null )
		{
			Log.Info( "Unable to reload Forsaken item definitions because TypeLibrary is null!" );
			return;
		}

		var resources = ResourceLibrary.GetAll<ItemResource>();

		foreach ( var resource in resources )
		{
			var type = TypeLibrary.GetType( resource.GetType() );

			if ( type == null )
			{
				Log.Error( $"Unable to find the TypeDescription for type {resource.GetType()}" );
				continue;
			}

			var attribute = type.GetAttribute<ItemClassAttribute>();
			if ( attribute == null ) continue;

			var itemType = TypeLibrary.GetType( attribute.Type );
			var instance = itemType.Create<InventoryItem>();

			if ( instance is IResourceItem a )
			{
				a.LoadResource( resource );
			}

			if ( string.IsNullOrEmpty( resource.UniqueId ) )
			{
				Log.Error( $"No unique id specified for item resource {resource.ItemName} {resource.ResourcePath}" );
				continue;
			}

			AddDefinition( resource.UniqueId, instance );
		}

		var types = TypeLibrary.GetTypes<InventoryItem>();

		foreach ( var type in types )
		{
			if ( !type.IsAbstract && !type.IsGenericType )
			{
				var instance = type.Create<InventoryItem>();
				if ( string.IsNullOrEmpty( instance.UniqueId ) ) continue;
				AddDefinition( instance.UniqueId, instance );
			}
		}
	}

	public static void AddDefinition( string uniqueId, InventoryItem definition )
	{
		if ( Definitions.ContainsKey( uniqueId ) )
		{
			Log.Error( $"Unable to add item definition for: {uniqueId}. Another item with this unique id already exists!" );
			return;
		}

		definition.IsValid = true;

		Definitions.Add( uniqueId, definition );
	}

	[Event.Hotload]
	private static void Hotloaded()
	{
		ReloadDefinitions();
	}

	[Event.Tick.Server]
	private static void ServerTick()
	{
		for ( var i = DirtyList.Count - 1; i >= 0; i-- )
		{
			var container = DirtyList[i];
			container.SendDirtyItems();
			container.IsDirty = false;
		}

		DirtyList.Clear();
	}

	[Event.Tick]
	private static void CheckOrphanedItems()
	{
		foreach ( var kv in Items )
		{
			var item = kv.Value;

			if ( !item.Parent.IsValid() && !item.WorldEntity.IsValid() )
			{
				OrphanedItems.Enqueue( kv.Key );
				item.IsValid = false;
			}
		}

		var totalOrphanedItems = 0;

		while ( OrphanedItems.Count > 0 )
		{
			var itemId = OrphanedItems.Dequeue();

			if ( Items.TryGetValue( itemId, out var item ) )
			{
				RemoveItem( item );
			}

			totalOrphanedItems++;
		}

		if ( totalOrphanedItems > 0 )
		{
			Log.Info( $"Removing {totalOrphanedItems} orphaned inventory items..." );
		}
	}
}
