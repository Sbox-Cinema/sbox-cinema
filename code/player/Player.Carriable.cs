using Sandbox;

namespace Cinema;

public partial class Player
{
	[Net, Predicted]
	public Carriable ActiveChild { get; set; }

	/// <summary>
	/// This isn't networked, but it's predicted. If it wasn't then when the prediction system
	/// re-ran the commands LastActiveChild would be the value set in a future tick, so ActiveEnd
	/// and ActiveStart would get called multiple times and out of order, causing all kinds of pain.
	/// </summary>
	[Predicted]
	Carriable LastActiveChild { get; set; }

	public virtual void SimulateActiveChild( IClient cl, Carriable child )
	{
		if ( LastActiveChild != child )
		{
			OnActiveChildChanged( LastActiveChild, child );
			LastActiveChild = child;
		}

		if ( !LastActiveChild.IsValid() )
			return;

		if ( LastActiveChild.IsAuthority )
		{
			LastActiveChild.Simulate( cl );
		}
	}

	/// <summary>
	/// Called when the Active child is detected to have changed
	/// </summary>
	public virtual void OnActiveChildChanged( Carriable previous, Carriable next )
	{
		if ( previous is Carriable previousBc )
		{
			previousBc?.ActiveEnd( this, previousBc.Owner != this );
		}

		if ( next is Carriable nextBc )
		{
			nextBc?.ActiveStart( this );
		}
	}

}
