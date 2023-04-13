using System;

namespace Cinema;

public struct AnglePosition
{
	public Angles Angle { get; set; } = new Angles();
	public Vector3 Position { get; set; } = new Vector3();

	public static readonly AnglePosition Zero = new();

	public AnglePosition() { }

	public AnglePosition( Angles angle, Vector3 pos )
	{
		Angle = angle;
		Position = pos;
	}

	public bool Equals( AnglePosition angPos )
	{
		return Angle == angPos.Angle && Position == angPos.Position;
	}

	public static AnglePosition operator +( AnglePosition x, AnglePosition y )
	{
		return new AnglePosition( x.Angle + y.Angle, x.Position + y.Position );
	}

	public static AnglePosition operator -( AnglePosition x, AnglePosition y )
	{
		return new AnglePosition( x.Angle - y.Angle, x.Position - y.Position );
	}

	public static AnglePosition operator -( AnglePosition x )
	{
		return new AnglePosition( x.Angle * -1, -x.Position );
	}

	public static bool operator ==( AnglePosition x, AnglePosition y )
	{
		return x.Equals( y );
	}

	public static bool operator !=( AnglePosition x, AnglePosition y )
	{
		return !x.Equals( y );
	}

	public override bool Equals( object obj )
	{
		if ( obj is not AnglePosition other )
		{
			return false;
		}

		return this.Angle == other.Angle && this.Position == other.Position;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine( Angle, Position );
	}
}
