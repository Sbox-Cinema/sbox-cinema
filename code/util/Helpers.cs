using System.Collections.Generic;
using System.Linq;

using Sandbox;

namespace Cinema;

public static class Helpers
{
	public static IEnumerable<(T item, int index)> WithIndex<T>( this IEnumerable<T> source )
	{
		return source.Select( ( item, index ) => (item, index) );
	}

	public static IList<T> Shuffle<T>( this IList<T> list )
	{
		var n = list.Count;
		while ( n > 1 )
		{
			n--;
			var k = Game.Random.Int( 0, n );
			var value = list[k];
			list[k] = list[n];
			list[n] = value;
		}

		return list;
	}
}
