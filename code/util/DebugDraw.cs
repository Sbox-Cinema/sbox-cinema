using Sandbox;
using System;

namespace Sandbox.Debug
{
	public struct DebugDraw
	{
		public static DebugDraw Once => new DebugDraw( Time.Delta * 1.5f, Color.Green, true );

		public static DebugDraw ForSeconds( float seconds ) => Once.WithDuration( seconds );

		public float Duration;
		public Color Color;
		public bool DepthTest;

		public DebugDraw( float duration, Color color, bool depth )
		{
			Duration = Time.Delta * 1.2f;
			Color = color;
			DepthTest = true;
		}

		public DebugDraw( float duration )
		{
			Duration = duration;
			Color = Color.Green;
			DepthTest = true;
		}

		public DebugDraw WithColor( Color color )
		{
			var t = this;
			t.Color = color;
			return t;
		}

		public DebugDraw WithSettings( float duration )
		{
			var t = this;
			t.Duration = duration;
			return t;
		}

		public DebugDraw WithDuration( float duration )
		{
			var t = this;
			t.Duration = duration;
			return t;
		}

		public DebugDraw WithSettings( float duration, Color color, bool depth )
		{
			return new DebugDraw( duration, color, depth );
		}

		public DebugDraw IgnoreDepth()
		{
			var t = this;
			t.DepthTest = false;
			return t;
		}

		public void Arrow( Vector3 startPos, Vector3 endPos, Vector3 up, float width = 8.0f )
		{
			var lineDir = (endPos - startPos).Normal;
			var sideDir = lineDir.Cross( up );
			var radius = width * 0.5f;

			var p1 = startPos - sideDir * radius;
			var p2 = endPos - lineDir * width - sideDir * radius;
			var p3 = endPos - lineDir * width - sideDir * width;
			var p4 = endPos;
			var p5 = endPos - lineDir * width + sideDir * width;
			var p6 = endPos - lineDir * width + sideDir * radius;
			var p7 = startPos + sideDir * radius;

			Line( p1, p2 );
			Line( p2, p3 );
			Line( p3, p4 );
			Line( p4, p5 );
			Line( p5, p6 );
			Line( p6, p7 );
		}

		public void Circle(
			Vector3 startPos,
			Rotation rot,
			float radius,
			int segments = 32,
			float degrees = 360
		)
		{
			var up = rot.Up;
			var right = rot.Right;

			float fsegPi = (degrees.DegreeToRadian()) / segments;

			Vector3 lp = default;

			for ( int i = 0; i <= segments; i++ )
			{
				var x = MathF.Sin( i * fsegPi ) * radius;
				var y = MathF.Cos( i * fsegPi ) * radius;

				var p = startPos + up * x + right * y;

				if ( i > 0 )
					Line( p, lp );

				lp = p;
			}
		}

		public void Circle(
			Vector3 startPos,
			Vector3 normal,
			float radius,
			int segments = 32,
			float degrees = 360
		)
		{
			Circle( startPos, Rotation.LookAt( normal ), radius, segments, degrees );
		}

		public void Line( Vector3 startPos, Vector3 endPos )
		{
			DebugOverlay.Line( startPos, endPos, Color, Duration, DepthTest );
		}
	}
}
