using System;

namespace Conna.Inventory;

/// <summary>
/// Use this attribute on a class derived from <see cref="ItemResource"/> to tell it what <see cref="IInventoryItem"/> class to use.
/// </summary>
public class ItemClassAttribute : Attribute
{
	public Type Type { get; private set; }

	public ItemClassAttribute( Type type )
	{
		Type = type;
	}
}
