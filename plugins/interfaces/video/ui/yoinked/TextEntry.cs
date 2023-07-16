using Sandbox;
using Sandbox.UI;
using Sandbox.UI.Construct;
using System;
using System.Globalization;
using System.Linq;

namespace CinemaTeam.Plugins.Media.Yoinked
{
	/// <summary>
	/// A <see cref="Panel"/> that the user can enter text into.
	/// </summary>
	[Library( "TextEntry" )]
	public partial class TextEntry : Panel, IInputControl
	{
		/// <summary>
		/// Called when the text of this text entry is changed.
		/// </summary>
		public Action<string> OnTextEdited { get; set; }

		/// <summary>
		/// The <see cref="Label"/> that contains the text of this text entry.
		/// TODO PAINDAY: This should be protected or internal, no?
		/// </summary>
		public Label Label { get; init; }

		bool _disabled;

		/// <summary>
		/// Is the text entry disabled?
		/// If disabled, will add a "disabled" class and prevent focus.
		/// </summary>
		public bool Disabled
		{
			get => _disabled;
			set
			{
				_disabled = value;
				AcceptsFocus = !value;
				SetClass( "disabled", value );
			}
		}

		/// <summary>
		/// Access to the raw text in the text entry.
		/// </summary>
		public string Text
		{
			get => Label.Text;
			set => Label.Text = value;
		}

		/// <summary>
		/// The value of the text entry. Returns <see cref="Text"/>, but does special logic when setting text.
		/// </summary>
		public string Value
		{
			get => Label.Text;
			set
			{
				// don't change the value
				// when we're editing it
				if ( HasFocus )
					return;

				Label.Text = value;
				if ( Numeric )
				{
					Label.Text = FixNumeric();
				}
			}
		}

		/// <inheritdoc cref="Label.TextLength"/>
		public int TextLength
		{
			get => Label.TextLength;
		}

		/// <inheritdoc cref="Label.CaretPosition"/>
		public int CaretPosition
		{
			get => Label.CaretPosition;
			set => Label.CaretPosition = value;
		}

		public override bool HasContent => true;

		/// <summary>
		/// Whether to allow automatic replacement of emoji codes with their actual unicode emoji characters. See <see cref="Emoji"/>.
		/// </summary>
		public bool AllowEmojiReplace { get; set; } = false;

		/// <summary>
		/// Allow <a href="https://en.wikipedia.org/wiki/Input_method">IME input</a> when this is focused.
		/// </summary>
		public override bool AcceptsImeInput => true;

		/// <summary>
		/// Affects formatting of the text when <see cref="Numeric"/> is enabled. Accepts any format that is supported by <see cref="float.ToString(string?)"/>. <a href="https://learn.microsoft.com/en-us/dotnet/standard/base-types/custom-numeric-format-strings">See examples here</a>.
		/// </summary>
		[Category( "Presentation" )]
		public string NumberFormat { get; set; } = null;

		/// <summary>
		/// Makes it possible to enter new lines into the text entry. (By pressing the Enter key, which no longer acts as the submit key)
		/// </summary>
		[Property]
		public bool Multiline { get; set; } = false;

		/// <summary>
		/// Color of the text cursor/caret, the blinking line at which newly typed characters are inserted.
		/// </summary>
		[Property]
		public Color CaretColor { get; set; } = Color.White;


		/// <summary>
		/// If we're numeric, this is the lowest numeric value allowed
		/// </summary>
		public float? MinValue { get; set; }

		/// <summary>
		/// If we're numeric, this is the highest numeric value allowed
		/// </summary>
		public float? MaxValue { get; set; }

		/// <summary>
		/// The <see cref="Label"/> that shows <see cref="Prefix"/> text.
		/// </summary>
		public Label PrefixLabel { get; protected set; }

		/// <summary>
		/// If set, will display given text before the text entry box.
		/// </summary>
		public string Prefix
		{
			get => PrefixLabel?.Text;
			set
			{
				if ( string.IsNullOrWhiteSpace( value ) )
				{
					PrefixLabel?.Delete();
					SetClass( "has-prefix", false );
					return;
				}

				PrefixLabel ??= Add.Label( value, "prefix-label" );
				PrefixLabel.Text = value;

				SetClass( "has-prefix", PrefixLabel != null );
			}
		}

		/// <summary>
		/// The <see cref="Label"/> that shows <see cref="Suffix"/> text.
		/// </summary>
		public Label SuffixLabel { get; protected set; }

		/// <summary>
		/// If set, will display given text after the text entry box.
		/// </summary>
		public string Suffix
		{
			get => SuffixLabel?.Text;
			set
			{
				if ( string.IsNullOrWhiteSpace( value ) )
				{
					SuffixLabel?.Delete();
					SetClass( "has-suffix", false );
					return;
				}

				SuffixLabel ??= Add.Label( value, "suffix-label" );
				SuffixLabel.Text = value;

				SetClass( "has-suffix", SuffixLabel != null );
			}
		}


		public TextEntry()
		{
			AcceptsFocus = true;
			AddClass( "textentry" );

			Label = Add.Label( "", "content-label" );
		}

		public override void OnPaste( string text )
		{
			if ( Label.HasSelection() )
			{
				Label.ReplaceSelection( "" );
			}

			var e = StringInfo.GetTextElementEnumerator( text );
			while ( e.MoveNext() )
			{
				if ( MaxLength.HasValue && TextLength >= MaxLength )
					break;

				var str = e.GetTextElement();

				if ( str.Any( x => !CanEnterCharacter( x ) ) )
					continue;

				Text ??= "";
				Label.InsertText( str, CaretPosition );
				Label.MoveCaretPos( 1 );

				if ( str.Length == 1 && str[0] == ':' )
				{
					RealtimeEmojiReplace();
				}
			}

			OnValueChanged();
		}

		public override string GetClipboardValue( bool cut )
		{
			var value = Label.GetClipboardValue( cut );

			if ( cut )
			{
				Label.ReplaceSelection( "" );
				OnValueChanged();
			}

			return value;
		}
		public override void OnButtonEvent( ButtonEvent e )
		{
			// dont' send to parent
			e.StopPropagation = true;
		}


		public override void OnButtonTyped( ButtonEvent e )
		{
			e.StopPropagation = true;

			//Log.Info( $"OnButtonTyped {button}" );
			var button = e.Button;

			if ( Label.HasSelection() && (button == "delete" || button == "backspace") )
			{
				Label.ReplaceSelection( "" );
				OnValueChanged();

				return;
			}

			if ( button == "delete" )
			{
				if ( CaretPosition < TextLength )
				{
					if ( e.HasCtrl )
					{
						Label.MoveToWordBoundaryRight( true );
						Label.ReplaceSelection( string.Empty );
						OnValueChanged();
						return;
					}

					Label.RemoveText( CaretPosition, 1 );
					OnValueChanged();
				}

				return;
			}

			if ( button == "backspace" )
			{
				if ( CaretPosition > 0 )
				{
					if ( e.HasCtrl )
					{
						Label.MoveToWordBoundaryLeft( true );
						Label.ReplaceSelection( string.Empty );
						OnValueChanged();
						return;
					}

					Label.MoveCaretPos( -1 );
					Label.RemoveText( CaretPosition, 1 );
					OnValueChanged();
				}

				return;
			}

			if ( button == "a" && e.HasCtrl )
			{
				Label.SelectionStart = 0;
				Label.SelectionEnd = TextLength;
				return;
			}

			if ( button == "home" )
			{
				if ( !e.HasCtrl )
				{
					Label.MoveToLineStart( e.HasShift );
				}
				else
				{
					Label.SetCaretPosition( 0, e.HasShift );
				}
				return;
			}

			if ( button == "end" )
			{
				if ( !e.HasCtrl )
				{
					Label.MoveToLineEnd( e.HasShift );
				}
				else
				{
					Label.SetCaretPosition( TextLength, e.HasShift );
				}
				return;
			}

			if ( button == "left" )
			{
				if ( !e.HasCtrl )
				{
					Label.MoveCaretPos( -1, e.HasShift );
				}
				else
				{
					Label.MoveToWordBoundaryLeft( e.HasShift );
				}
				return;
			}

			if ( button == "right" )
			{
				if ( !e.HasCtrl )
				{
					Label.MoveCaretPos( 1, e.HasShift );
				}
				else
				{
					Label.MoveToWordBoundaryRight( e.HasShift );
				}
				return;
			}

			if ( button == "down" || button == "up" )
			{
				if ( AutoCompletePanel != null )
				{
					AutoCompletePanel.MoveSelection( button == "up" ? -1 : 1 );
					AutoCompleteSelectionChanged();
					return;
				}

				//
				// We have history items, autocomplete using those
				//
				if ( string.IsNullOrEmpty( Text ) && AutoCompletePanel == null && History.Count > 0 )
				{
					UpdateAutoComplete( History.ToArray() );

					// select last item
					AutoCompletePanel.MoveSelection( -1 );
					AutoCompleteSelectionChanged();

					return;
				}

				Label.MoveCaretLine( button == "up" ? -1 : 1, e.HasShift );
				return;
			}

			if ( button == "enter" || button == "pad_enter" )
			{
				if ( Multiline )
				{
					OnKeyTyped( '\n' );
					return;
				}

				if ( AutoCompletePanel != null && AutoCompletePanel.SelectedChild != null )
				{
					DestroyAutoComplete();
				}

				Blur();
				CreateEvent( "onsubmit", Text );
				return;
			}

			if ( button == "escape" )
			{
				if ( AutoCompletePanel != null )
				{
					AutoCompleteCancel();
					return;
				}

				Blur();
				CreateEvent( "oncancel" );
				return;
			}

			if ( button == "tab" )
			{
				if ( AutoCompletePanel != null )
				{
					AutoCompletePanel.MoveSelection( e.HasShift ? -1 : 1 );
					AutoCompleteSelectionChanged();
					return;
				}
			}

			base.OnButtonTyped( e );
		}

		protected override void OnMouseDown( MousePanelEvent e )
		{
			var pos = Label.GetLetterAtScreenPosition( Mouse.Position );

			Label.SelectionStart = 0;
			Label.SelectionEnd = 0;

			if ( pos >= 0 )
			{
				Label.SetCaretPosition( pos );
			}

			e.StopPropagation();
		}

		protected override void OnMouseUp( MousePanelEvent e )
		{
			SelectingWords = false;

			var pos = Label.GetLetterAtScreenPosition( Mouse.Position );
			if ( Label.SelectionEnd > 0 ) pos = Label.SelectionEnd;
			Label.CaretPosition = pos.Clamp( 0, TextLength );

			e.StopPropagation();
		}

		protected override void OnMouseMove( MousePanelEvent e )
		{
			base.OnMouseMove( e );
			e.StopPropagation();
		}

		protected override void OnFocus( PanelEvent e )
		{
			UpdateAutoComplete();
		}

		protected override void OnBlur( PanelEvent e )
		{
			//UpdateAutoComplete();

			if ( Numeric )
			{
				Text = FixNumeric();
			}
		}

		private bool SelectingWords = false;
		protected override void OnDoubleClick( MousePanelEvent e )
		{
			if ( e.Button == "mouseleft" )
			{
				Label.SelectWord( Label.GetLetterAtScreenPosition( Mouse.Position ) );
				SelectingWords = true;
			}
		}

		public override void OnKeyTyped( char k )
		{
			if ( !CanEnterCharacter( k ) )
				return;

			if ( MaxLength.HasValue && TextLength >= MaxLength )
				return;

			if ( Label.HasSelection() )
			{
				Label.ReplaceSelection( k.ToString() );
			}
			else
			{
				Text ??= "";
				Label.InsertText( k.ToString(), CaretPosition );
				Label.MoveCaretPos( 1 );
			}

			if ( k == ':' )
			{
				RealtimeEmojiReplace();
			}

			OnValueChanged();
		}


		public override void DrawContent( ref RenderState state )
		{
			Label.ShouldDrawSelection = HasFocus;

			var blinkRate = 0.8f;

			if ( HasFocus )
			{
				var blink = (TimeSinceNotInFocus * blinkRate) % blinkRate < (blinkRate * 0.5f);
				var caret = Label.GetCaretRect( CaretPosition );
				caret.Right += 0.4f;
				caret.Left -= 0.4f;

				var color = CaretColor;
				color.a *= blink ? 1.0f : 0.05f;

				Graphics.DrawRoundedRectangle( caret, color );
			}
		}

		void RealtimeEmojiReplace()
		{
			if ( !AllowEmojiReplace )
				return;

			if ( CaretPosition == 0 )
				return;

			string lookup = null;
			var arr = StringInfo.ParseCombiningCharacters( Text );
			var caretStringPosition = arr[CaretPosition - 1];

			for ( int i = caretStringPosition - 2; i >= 0; i-- )
			{
				var c = Text[i];

				if ( char.IsWhiteSpace( c ) )
					return;

				if ( c == ':' )
				{
					lookup = Text.Substring( i, caretStringPosition - i + 1 );
					break;
				}

				if ( i == 0 )
					return;
			}

			if ( lookup == null )
				return;

			var replace = Emoji.FindEmoji( lookup );
			if ( replace == null )
				return;

			CaretPosition -= lookup.Length - 1; // set this first so we don't get abused by CaretSanity
			Text = Text.Replace( lookup, replace );
		}

		/// <summary>
		/// Called when the text entry's value changes.
		/// </summary>
		public virtual void OnValueChanged()
		{
			UpdateAutoComplete();
			UpdateValidation();

			if ( Numeric )
			{
				// with numberic, we don't ever want to
				// send out invalid values to binds
				var text = FixNumeric();
				CreateEvent( "onchange" );
				CreateValueEvent( "value", text );
				OnTextEdited?.Invoke( text );
			}
			else
			{
				CreateEvent( "onchange" );
				CreateValueEvent( "value", Text );
				OnTextEdited?.Invoke( Text );
			}
		}

		/// <summary>
		/// Keep tabs of when we were focused so we can flash the caret relative to that time.
		/// We want the caret to be visible immediately on focus
		/// </summary>
		RealTimeSince TimeSinceNotInFocus;

		public override void Tick()
		{
			base.Tick();

			SetClass( "is-multiline", Multiline );
			SetClass( "has-placeholder", string.IsNullOrEmpty( Text ) && PlaceholderLabel != null );

			if ( Label != null )
				Label.Multiline = Multiline;

			if ( !HasFocus )
				TimeSinceNotInFocus = 0;
		}

		public override void SetProperty( string name, string value )
		{
			base.SetProperty( name, value );

			if ( name == "placeholder" )
			{
				Placeholder = value;
			}

			if ( name == "numeric" )
			{
				Numeric = value.ToBool();
			}

			if ( name == "format" )
			{
				NumberFormat = value;
			}

			if ( name == "value" && !HasFocus )
			{
				//
				// When setting tha value, and we're numeric, convert it to a number
				//
				if ( Numeric )
				{
					if ( !float.TryParse( value, out var floatValue ) )
						return;

					Text = floatValue.ToString( NumberFormat );
					return;
				}

				Text = value;
			}

			if ( name == "disabled" )
			{
				Disabled = value.ToBool();
			}
		}

		/// <summary>
		/// Called to ensure the <see cref="Text"/> is absolutely in the correct format, in this case - a valid number format.
		/// </summary>
		/// <returns>The correctly formatted version of <see cref="Text"/>.</returns>
		public virtual string FixNumeric()
		{
			if ( !float.TryParse( Text, out var floatValue ) )
			{
				var val = 0.0f.Clamp( MinValue ?? floatValue, MaxValue ?? floatValue );
				return val.ToString();
			}

			floatValue = floatValue.Clamp( MinValue ?? floatValue, MaxValue ?? floatValue );
			return floatValue.ToString( NumberFormat );
		}

		protected override void OnDragSelect( SelectionEvent e )
		{
			Label.ShouldDrawSelection = true;

			var tl = new Vector2( e.SelectionRect.Left, e.SelectionRect.Top );
			var br = new Vector2( e.SelectionRect.Right, e.SelectionRect.Bottom );
			Label.SelectionStart = Label.GetLetterAtScreenPosition( tl );
			Label.SelectionEnd = Label.GetLetterAtScreenPosition( br );

			if ( SelectingWords )
			{
				var boundaries = Label.GetWordBoundaryIndices();

				var left = boundaries.LastOrDefault( x => x < Label.SelectionStart );
				var right = boundaries.FirstOrDefault( x => x > Label.SelectionEnd );

				left = Math.Min( left, Label.SelectionStart );
				right = Math.Max( right, Label.SelectionEnd );

				Label.SelectionStart = left;
				Label.SelectionEnd = right;
			}
		}
	}

	namespace Construct
	{
		public static class TextEntryConstructor
		{
			/// <summary>
			/// Creates and returns a text entry with predefined text already entered.
			/// </summary>
			public static TextEntry TextEntry( this PanelCreator self, string text = "" )
			{
				var control = self.panel.AddChild<TextEntry>();
				control.Text = text;

				return control;
			}
		}
	}

}
