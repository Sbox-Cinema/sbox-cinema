using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	/// <summary>
	/// A panel containing an icon, typically a <a href="https://fonts.google.com/icons">material icon</a>.
	/// </summary>
	[Library( "IconPanel" ), Alias( "icon", "i" ),]
	public class IconPanel : Label
	{
		public IconPanel()
		{
			AddClass( "iconpanel" );
		}
	}

	namespace Construct
	{
		public static class IconPanelConstructor
		{
			/// <summary>
			/// Create and return an icon (panel) with given icon and optionally given CSS classes.
			/// </summary>
			public static IconPanel CreateIcon( this PanelCreator self, string icon, string classes = null )
			{
				var control = self.panel.AddChild<IconPanel>();

				if ( icon != null )
					control.Text = icon;

				if ( classes != null )
					control.AddClass( classes );

				return control;
			}
		}
	}

}
