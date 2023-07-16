
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	public partial class TextEntry
	{
		internal List<string> History { get; set; } = new List<string>();

		/// <summary>
		/// Maximum amount of items <see cref="AddToHistory"/> will keep.
		/// Oldest items will be discarded as new ones are added.
		/// </summary>
		public int HistoryMaxItems { get; set; } = 30;

		string _historyCookie;
		/// <summary>
		/// If set, the history of this text entry will be stored and restored using this key in the local <see cref="Cookie"/>.
		/// </summary>
		public string HistoryCookie
		{
			get => _historyCookie;
			set
			{
				if ( _historyCookie == value ) return;

				_historyCookie = value;

				if ( string.IsNullOrEmpty( _historyCookie ) )
					return;
				
				History = Cookie.Get( value, History );
			}
		}

		/// <summary>
		/// Add given string to the history of this text entry. 
		/// The history can be accessed by the player by pressing up and down arrows with an empty text entry.
		/// </summary>
		public void AddToHistory( string str )
		{
			History.RemoveAll( x => x == str );
			History.Add( str );

			if ( HistoryMaxItems > 0 )
			{
				while ( History.Count > HistoryMaxItems )
				{
					History.RemoveAt( 0 );
				}
			}

			if ( !string.IsNullOrEmpty( HistoryCookie ) )
			{
				Cookie.Set( HistoryCookie, History );
			}
		}

		/// <summary>
		/// Clear the input history that was previously added via <see cref="AddToHistory"/>.
		/// </summary>
		public void ClearHistory()
		{
			History.Clear();

			if ( !string.IsNullOrEmpty( HistoryCookie ) )
			{
				Cookie.Set( HistoryCookie, History );
			}
		}
	}
}
