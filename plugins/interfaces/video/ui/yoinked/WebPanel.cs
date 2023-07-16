using Sandbox;
using Sandbox.UI;
using System;

namespace CinemaTeam.Plugins.Media.Yoinked;

/// <summary>
/// A panel that displays an interactive web page.
/// </summary>
public class WebPanel : Panel
{
	/// <summary>
	/// Access to the HTML surface to change URL, etc.
	/// </summary>
	public WebSurface Surface { get; private set; }

	public WebPanel()
	{
		AcceptsFocus = true;

		Surface = Game.CreateWebSurface();
		Surface.Size = Box.Rect.Size;
		Surface.OnTexture = BrowserDataChanged;
	}

	Texture sufaceTexture;


	/// <summary>
	/// The texture has changed
	/// </summary>
	private void BrowserDataChanged( ReadOnlySpan<byte> span, Vector2 size )
	{
		//
		// Create or Recreate the texture if it changed
		//
		if ( sufaceTexture == null || sufaceTexture.Size != size )
		{
			sufaceTexture?.Dispose();
			sufaceTexture = null;

			sufaceTexture = Texture.Create( (int)size.x, (int)size.y, ImageFormat.BGRA8888 )
										.WithName( "WebPanel" )
										.Finish();

			Style.SetBackgroundImage( sufaceTexture );
		}

		//
		// Update with thw new data
		//
		sufaceTexture.Update( span, 0, 0, (int)size.x, (int)size.y );
	}

	protected override void OnFocus( PanelEvent e ) => Surface.HasKeyFocus = true;
	protected override void OnBlur( PanelEvent e ) => Surface.HasKeyFocus = false;
	public override void OnMouseWheel( float value )
	{
		Surface.TellMouseWheel( (int)value * -40 );
	}

	protected override void OnMouseDown( MousePanelEvent e )
	{
		Surface.TellMouseButton( e.MouseButton, true );
		e.StopPropagation();
	}

	protected override void OnMouseUp( MousePanelEvent e )
	{
		Surface.TellMouseButton( e.MouseButton, false );
		e.StopPropagation();
	}

	public override void OnKeyTyped( char k ) => Surface.TellChar( k, KeyboardModifiers.None );

	public override void OnButtonEvent( ButtonEvent e )
	{
		Surface.TellKey( (uint)e.VirtualKey, e.KeyboardModifiers, e.Pressed );
		//e.StopPropagation();
	}

	public override void OnLayout( ref Rect layoutRect )
	{
		Surface.Size = Box.Rect.Size;
		Surface.ScaleFactor = ScaleToScreen;
	}

	protected override void OnMouseMove( MousePanelEvent e )
	{
		Surface.TellMouseMove( e.LocalPosition );
		Style.Cursor = Surface.Cursor;
	}

	public override void OnDeleted()
	{
		base.OnDeleted();

		Surface?.Dispose();
		Surface = null;
	}

}
