using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;
using System.Linq;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	public partial class Popup
	{
		static List<Popup> AllPopups = new();

		/// <summary>
		/// Close all <see cref="Popup"/> instances except for the given one.
		/// </summary>
		/// <param name="exceptThisOne">If set, this popup with not be closed.</param>
		public static void CloseAll( Panel exceptThisOne = null )
		{
			foreach ( var panel in AllPopups.ToArray() )
			{
				if ( panel == exceptThisOne ) continue;

				panel.Delete();
			}
		}

		/// <summary>
		/// Event listener for <c>ui.closepopups</c>.
		/// PAINDAY TODO: Make internal?
		/// </summary>
		[Event( "ui.closepopups" )]
		public static void ClosePopupsEvent( object obj )
		{
			Popup floater = null;

			if ( obj is Panel panel )
			{
				floater = panel.AncestorsAndSelf.OfType<Popup>().FirstOrDefault();
			}

			CloseAll( floater );
		}
	}
}
