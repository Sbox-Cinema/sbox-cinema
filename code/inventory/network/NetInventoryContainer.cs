using Sandbox;

namespace Conna.Inventory;

/// <summary>
/// A networked inventory container. This makes it easy to network using <see cref="NetAttribute"/>.
/// </summary>
public class NetInventoryContainer : BaseNetworkable, INetworkSerializer, IValid
{
	public InventoryContainer Value { get; private set; }

	public bool IsValid => Value.IsValid();
	public uint Version { get; private set; }

	public NetInventoryContainer()
	{

	}

	public NetInventoryContainer( InventoryContainer container )
	{
		Value = container;
	}

	public bool Is( InventoryContainer container )
	{
		return container == Value;
	}

	public bool Is( NetInventoryContainer container )
	{
		return container == this;
	}

	public void Read( ref NetRead read )
	{
		var version = read.Read<uint>();
		var containerId = read.Read<ulong>();
		var totalBytes = read.Read<int>();
		var output = new byte[totalBytes];
		read.ReadUnmanagedArray( output );

		if ( Version == version ) return;

		var container = InventorySystem.Find( containerId );
		if ( container.IsValid() )
		{
			Value = container;
			return;
		}

		Value = InventoryContainer.Deserialize( output );

		Version = version;
	}

	public void Write( NetWrite write )
	{
		var serialized = Value.Serialize();
		write.Write( ++Version );
		write.Write( Value.ContainerId );
		write.Write( serialized.Length );
		write.WriteUnmanagedArray( serialized );
	}
}
