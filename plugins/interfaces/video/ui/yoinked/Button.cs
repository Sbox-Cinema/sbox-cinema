using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using CinemaTeam.Plugins.Media.Yoinked.Construct;
using System;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	/// <summary>
	/// A simple button <see cref="Panel"/>.
	/// </summary>
	[Library( "Button" )]
	public class Button : Panel
	{
		/// <summary>
		/// The <see cref="Label"/> that displays <see cref="Text"/>.
		/// </summary>
		protected Label TextLabel;

		/// <summary>
		/// The <see cref="IconPanel"/> that displays <see cref="Icon"/>.
		/// </summary>
		protected IconPanel IconPanel;

		public Button()
		{
			AddClass( "button" );
		}

		public Button( string text ) : this()
		{
			if ( text != null )
				Text = text;
		}

		public Button( string text, string icon ) : this()
		{
			if ( icon != null )
				Icon = icon;

			if ( text != null )
				Text = text;
		}

		public Button( string text, string icon, Action onClick ) : this( text, icon )
		{
			AddEventListener( "onclick", onClick );
		}

		/// <summary>
		/// Text for the button.
		/// </summary>
		public string Text
		{
			get => TextLabel?.Text;
			set
			{
				TextLabel ??= Add.Label( value, "button-label" );
				TextLabel.Text = value;
			}
		}

		/// <summary>
		/// Deletes the <see cref="Text"/>.
		/// </summary>
		public void DeleteText()
		{
			if ( TextLabel == null ) return;

			TextLabel?.Delete();
			TextLabel = null;
		}

		/// <summary>
		/// Icon for the button.
		/// </summary>
		public string Icon
		{
			get => IconPanel?.Text;
			set
			{
				if ( string.IsNullOrEmpty( value ) )
				{
					DeleteIcon();
					return;
				}

				IconPanel ??= Add.CreateIcon( value );
				IconPanel.Text = value;
				SetClass( "has-icon", IconPanel != null );
			}
		}

		/// <summary>
		/// Deletes the <see cref="Icon"/>.
		/// </summary>
		public void DeleteIcon()
		{
			if ( IconPanel == null ) return;

			IconPanel?.Delete();
			IconPanel = null;
			RemoveClass( "has-icon" );
		}

		/// <summary>
		/// Set the text for the button. Calls <c>Text = value</c>
		/// </summary>
		public virtual void SetText( string text )
		{
			Text = text;
		}

		/// <summary>
		/// Imitate the button being clicked.
		/// </summary>
		public void Click()
		{
			CreateEvent( new MousePanelEvent( "onclick", this, "mouseleft" ) );
		}

		public override void SetProperty( string name, string value )
		{
			switch ( name )
			{
				case "text":
					{
						SetText( value );
						return;
					}

				case "html":
					{
						SetText( value );
						return;
					}

				case "icon":
					{
						Icon = value;
						return;
					}

				case "active":
					{
						SetClass( "active", value.ToBool() );
						return;
					}
			}

			base.SetProperty( name, value );
		}

		public override void SetContent( string value )
		{
			SetText( value?.Trim() ?? "" );
		}
	}

	namespace Construct
	{
		public static class ButtonConstructor
		{
			/// <summary>
			/// Create a button with given text and on-click action.
			/// </summary>
			public static Button CreateButton( this PanelCreator self, string text, Action onClick = null )
			{
				var b = new Button( text, null, onClick );
				self.panel.AddChild( b );
				return b;
			}

			/// <summary>
			/// Create a button with given text, CSS class name and on-click action.
			/// </summary>
			public static Button CreateButton( this PanelCreator self, string text, string className, Action onClick = null )
			{
				var control = self.panel.AddChild<Button>();

				if ( text != null )
					control.SetText( text );

				if ( onClick != null )
					control.AddEventListener( "onclick", onClick );

				if ( className != null )
					control.AddClass( className );

				return control;
			}

			/// <summary>
			/// Create a button with given text, icon, CSS class name and on-click action.
			/// </summary>
			public static Button CreateButtonWithIcon( this PanelCreator self, string text, string icon, string className, Action onClick = null )
			{
				var control = self.panel.AddChild<Button>();

				if ( icon != null )
					control.Icon = icon;

				if ( text != null )
					control.SetText( text );

				if ( onClick != null )
					control.AddEventListener( "onclick", onClick );

				if ( className != null )
					control.AddClass( className );

				return control;
			}

			/// <summary>
			/// Create a button with given text that runs a console command on click.
			/// </summary>
			public static Button ButtonWithConsoleCommand( this PanelCreator self, string text, string command )
			{
				return CreateButton( self, text, () => ConsoleSystem.Run( command ) );
			}
		}
	}

}
