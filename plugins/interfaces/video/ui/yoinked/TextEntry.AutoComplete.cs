using Sandbox;
using Sandbox.UI;
using System;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	public partial class TextEntry
	{
		/// <summary>
		/// If you hook a method up here we'll do autocomplete on it.
		/// Return a list if strings for given string input.
		/// </summary>
		public Func<string, object[]> AutoComplete { get; set; }

		internal Popup AutoCompletePanel;

		/// <summary>
		/// Open the auto complete popup with values from <see cref="AutoComplete"/>.
		/// Close the popup if we have no auto complete entries.
		/// </summary>
		public void UpdateAutoComplete()
		{
			if ( AutoComplete == null )
			{
				DestroyAutoComplete();
				return;
			}

			var results = AutoComplete( Text );
			if ( results == null || results.Length == 0 )
			{
				DestroyAutoComplete();
				return;
			}

			UpdateAutoComplete( results );
		}

		/// <summary>
		/// Open the auto complete popup with given values.
		/// </summary>
		public void UpdateAutoComplete( object[] options )
		{
			if ( AutoCompletePanel == null || AutoCompletePanel.IsDeleting )
			{
				AutoCompletePanel = new Popup( this, Popup.PositionMode.AboveLeft, 8 );
				AutoCompletePanel.AddClass( "autocomplete" );
				AutoCompletePanel.SkipTransitions();
			}

			AutoCompletePanel.DeleteChildren( true );
			AutoCompletePanel.UserData = Text;

			foreach ( var r in options )
			{
				var b = AutoCompletePanel.AddOption( r.ToString(), () => AutoCompleteSelected( r ) );
				b.UserData = r;
			}
		}

		/// <summary>
		/// Close and delete the auto complete popup panel.
		/// </summary>
		public virtual void DestroyAutoComplete()
		{
			AutoCompletePanel?.Delete();
			AutoCompletePanel = null;
		}

		void AutoCompleteSelected( object obj )
		{
			Text = obj.ToString();
			Focus();
			OnValueChanged();

			Label.MoveToLineEnd();
		}

		/// <summary>
		/// Auto complete selection has changed. Update the text entry text to that selected value.
		/// </summary>
		protected virtual void AutoCompleteSelectionChanged()
		{
			var selected = AutoCompletePanel.SelectedChild;
			if ( selected == null ) return;

			Text = selected.UserData.ToString();
			Label.MoveToLineEnd();
		}

		/// <summary>
		/// Auto complete was canceled, restore text to what it was before we started, and remove the auto complete popup.
		/// </summary>
		protected virtual void AutoCompleteCancel()
		{
			Text = AutoCompletePanel.UserData.ToString();
			DestroyAutoComplete();
		}
	}
}
