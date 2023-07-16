using Sandbox;
using CinemaTeam.Plugins.Media.Yoinked.Construct;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	public partial class TextEntry
	{
		/// <summary>
		/// The <see cref="UI.IconPanel"/> that displays <see cref="Icon"/>
		/// </summary>
		public IconPanel IconPanel { get; protected set; }

		/// <summary>
		/// If set, will display a <a href="https://fonts.google.com/icons">material icon</a> at the end of the text entry.
		/// </summary>
		[Property]
		public string Icon
		{
			get => IconPanel?.Text;
			set
			{
				if ( string.IsNullOrEmpty( value ) )
				{
					IconPanel?.Delete( true );
				}
				else
				{
					IconPanel ??= Add.CreateIcon( value );
					IconPanel.Text = value;
				}

				SetClass( "has-icon", IconPanel != null );
			}
		}
	}
}
