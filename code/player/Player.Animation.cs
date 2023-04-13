using Sandbox;
using static Sandbox.Event;

namespace Cinema;


public partial class Player
{

	public virtual void TickAnimation()
	{
		// where should we be rotated to
		var turnSpeed = 0.02f;

		Rotation rotation;

		// If we're a bot, spin us around 180 degrees.
		if ( Client.IsBot )
			rotation = LookInput.WithYaw( LookInput.yaw + 180f ).ToRotation();
		else
			rotation = LookInput.ToRotation();

		if ( BodyController == null ) return;

		var wishVelocity = BodyController.GetWishVelocity();

		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		Rotation = Rotation.Slerp(
			Rotation,
			idealRotation,
			wishVelocity.Length * Time.Delta * turnSpeed
		);
		Rotation = Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( this );

		animHelper.WithWishVelocity( wishVelocity );
		animHelper.WithVelocity( BodyController.Velocity );
		animHelper.WithLookAt( EyePosition + EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp(
			animHelper.DuckLevel,
			BodyController.GetMechanic<CrouchMechanic>()?.IsActive == true ? 1 : 0,
			Time.Delta * 10.0f
		);
		animHelper.VoiceLevel =
			(Game.IsClient && Client.IsValid())
				? Client.Voice.LastHeard < 0.5f
					? Client.Voice.CurrentLevel
					: 0.0f
				: 0.0f;
		animHelper.IsGrounded = GroundEntity != null;
		animHelper.IsSwimming = this.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		if ( BodyController.GetMechanic<JumpMechanic>()?.Jumping == true )
			animHelper.TriggerJump();

		if ( ActiveChild is Carriable carry )
		{
			carry.SimulateAnimator( animHelper );
		}
		else
		{
			animHelper.HoldType = CitizenAnimationHelper.HoldTypes.None;
			animHelper.AimBodyWeight = 0.5f;
		}
	}
}
