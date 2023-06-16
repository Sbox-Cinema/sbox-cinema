using Sandbox;

namespace Conna.Inventory;

/// <summary>
/// A networked inventory item. This makes it easy to network using <see cref="NetAttribute"/>.
/// </summary>
public class NetInventoryItem : BaseNetworkable, INetworkSerializer, IValid
{
	public IInventoryItem Value { get; private set; }

	public bool IsValid => Value.IsValid();
	public uint Version { get; private set; }

	public NetInventoryItem()
	{

	}

	public NetInventoryItem( IInventoryItem item )
	{
		Value = item;
	}

	public void Read( ref NetRead read )
	{
		var version = read.Read<uint>();
		var itemId = read.Read<ulong>();
		var totalBytes = read.Read<int>();
		var output = new byte[totalBytes];
		read.ReadUnmanagedArray( output );

		if ( Version == version ) return;

		var item = InventorySystem.FindInstance( itemId );
		if ( item.IsValid() )
		{
			Value = item;
			return;
		}

		Value = InventoryItem.Deserialize( output );
		Version = version;
	}

	public void Write( NetWrite write )
	{
		var serialized = Value.Serialize();
		write.Write( ++Version );
		write.Write( Value.ItemId );
		write.Write( serialized.Length );
		write.Write( serialized );
	}
}
