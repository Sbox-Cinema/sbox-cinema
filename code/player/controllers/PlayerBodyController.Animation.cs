using Sandbox;

namespace Cinema;


public partial class PlayerBodyController
{

	public virtual void SimulateAnimation()
	{
		// where should we be rotated to
		var turnSpeed = 0.02f;

		Rotation rotation;

		// If we're a bot, spin us around 180 degrees.
		if ( Entity.Client.IsBot )
			rotation = Entity.LookInput.WithYaw( Entity.LookInput.yaw + 180f ).ToRotation();
		else
			rotation = Entity.LookInput.ToRotation();

		var wishVelocity = GetWishVelocity();

		var idealRotation = Rotation.LookAt( rotation.Forward.WithZ( 0 ), Vector3.Up );
		Entity.Rotation = Rotation.Slerp(
			Entity.Rotation,
			idealRotation,
			wishVelocity.Length * Time.Delta * turnSpeed
		);
		Entity.Rotation = Entity.Rotation.Clamp( idealRotation, 45.0f, out var shuffle ); // lock facing to within 45 degrees of look direction

		CitizenAnimationHelper animHelper = new CitizenAnimationHelper( Entity );

		animHelper.WithWishVelocity( wishVelocity );
		animHelper.WithVelocity( Velocity );
		animHelper.WithLookAt( Entity.EyePosition + Entity.EyeRotation.Forward * 100.0f, 1.0f, 1.0f, 0.5f );
		animHelper.AimAngle = rotation;
		animHelper.FootShuffle = shuffle;
		animHelper.DuckLevel = MathX.Lerp(
			animHelper.DuckLevel,
			GetMechanic<CrouchMechanic>()?.IsActive == true ? 1 : 0,
			Time.Delta * 10.0f
		);
		animHelper.VoiceLevel =
			(Game.IsClient && Entity.Client.IsValid())
				? Entity.Client.Voice.LastHeard < 0.5f
					? Entity.Client.Voice.CurrentLevel
					: 0.0f
				: 0.0f;
		animHelper.IsGrounded = GroundEntity != null;
		animHelper.IsSwimming = Entity.GetWaterLevel() >= 0.5f;
		animHelper.IsWeaponLowered = false;

		if ( GetMechanic<JumpMechanic>()?.Jumping == true )
			animHelper.TriggerJump();

		if ( Entity.ActiveChild is Carriable carry )
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
