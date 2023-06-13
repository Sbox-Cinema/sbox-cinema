using Sandbox;
using System;
using System.IO;

namespace Conna.Inventory;

public static partial class BinaryReaderExtension
{
	/// <summary>
	/// Read an <see cref="IInventoryItem"/> from a reader.
	/// </summary>
	/// <param name="buffer"></param>
	/// <returns></returns>
	public static IInventoryItem ReadInventoryItem( this BinaryReader buffer )
	{
		var uniqueId = buffer.ReadString();

		if ( !string.IsNullOrEmpty( uniqueId ) )
		{
			var stackSize = buffer.ReadUInt16();
			var itemId = buffer.ReadUInt64();
			var slotId = buffer.ReadUInt16();

			var instance = InventorySystem.CreateItem( uniqueId, itemId );

			if ( instance != null )
			{
				instance.StackSize = stackSize;
				instance.SlotId = slotId;
				instance.Read( buffer );
			}

			return instance;
		}
		else
		{
			return null;
		}
	}

	public static void ReadWrapped( this BinaryReader self, Action<BinaryReader> wrapper )
	{
		var length = self.ReadInt32();
		var data = self.ReadBytes( length );

		using ( var stream = new MemoryStream( data ) )
		{
			using ( var reader = new BinaryReader( stream ) )
			{
				try
				{
					wrapper( reader );
				}
				catch ( Exception e )
				{
					Log.Error( e );
				}
			}
		}
	}

	/// <summary>
	/// Read an inventory container and optionally into an existing <see cref="InventoryContainer"/>.
	/// </summary>
	/// <param name="buffer"></param>
	/// <param name="target"></param>
	/// <returns></returns>
	public static InventoryContainer ReadInventoryContainer( this BinaryReader buffer, InventoryContainer target = null )
	{
		var typeName = buffer.ReadString();
		var parentItemId = buffer.ReadUInt64();
		var containerId = buffer.ReadUInt64();
		var slotLimit = buffer.ReadUInt16();
		var entity = buffer.ReadEntity();

		var container = target ?? InventorySystem.Find( containerId );

		if ( container == null )
		{
			var type = TypeLibrary.GetType( typeName );

			if ( type == null )
			{
				Log.Error( $"Unable to create an inventory container with unknown type id ({typeName})!" );
				return null;
			}

			container = type.Create<InventoryContainer>();
			container.SetEntity( entity );
			container.SetParent( parentItemId );
			container.SetSlotLimit( slotLimit );
			InventorySystem.Register( container, containerId );
		}
		else
		{
			container.SetEntity( entity );
			container.SetParent( parentItemId );
			container.SetSlotLimit( slotLimit );
		}

		if ( target.IsValid() )
		{
			InventorySystem.ReassignId( target, containerId );
		}

		for ( var i = 0; i < slotLimit; i++ )
		{
			var isValid = buffer.ReadBoolean();

			if ( isValid )
			{
				var item = buffer.ReadInventoryItem();
				item.IsValid = true;
				item.Parent = container;

				if ( Game.IsServer )
					container.Replace( (ushort)i, item );
				else
					container.ItemList[i] = item;
			}
			else
			{
				if ( Game.IsServer )
					container.ClearSlot( (ushort)i );
				else
					container.ItemList[i] = null;
			}
		}

		container.Deserialize( buffer );

		return container;
	}

}
