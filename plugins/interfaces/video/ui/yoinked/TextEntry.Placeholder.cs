using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	public partial class TextEntry
	{
		internal Label PlaceholderLabel;

		/// <summary>
		/// Text to display when the text entry is empty. Typically a very short description of the expected contents or function of the text entry.
		/// </summary>
		public string Placeholder
		{
			get => PlaceholderLabel?.Text;
			set
			{
				if ( string.IsNullOrEmpty( value ) )
				{
					PlaceholderLabel?.Delete();
					PlaceholderLabel = null;
					return;
				}

				if ( PlaceholderLabel == null )
					PlaceholderLabel = Add.Label( value, "placeholder" );

				PlaceholderLabel.Text = value;
			}
		}
	}
}
