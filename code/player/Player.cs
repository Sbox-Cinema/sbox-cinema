using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

public partial class Player : AnimatedEntity, IEyes
{
    //This doesn't need to be public
    ClothingContainer clothing = new();

    [BindComponent]
    public PlayerBodyController BodyController { get; }

    public IEnumerable<PlayerController> PlayerControllers => Components.GetAll<PlayerController>();

    public PlayerController ActiveController => PlayerControllers.FirstOrDefault((e) => e.Active);

    public static Model PlayerModel = Model.Load("models/citizen/citizen.vmdl");

    [Net, Predicted]
    public bool ThirdPersonCamera { get; set; }

    public Player()
    {

    }

    public Player( IClient cl ) : this()
    {
        clothing.LoadFromClient( cl );
    }

    /// <summary>
    /// Called when the entity is first created
    /// </summary>
    public override void Spawn()
    {
        Model = PlayerModel;

        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;
        EnableDrawing = true;

        MoveToSpawnLocation();
        CreateHull();
        SetupBodyController();

        /*Predictable = true;
        // Default properties
        EnableDrawing = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;
        EnableLagCompensation = true;
        EnableHitboxes = true;*/

        Tags.Add("player");
    }

    protected void MoveToSpawnLocation()
    {
        var spawnpoint = All.OfType<SpawnPoint>().OrderBy(x => Guid.NewGuid()).FirstOrDefault();

        if ( spawnpoint != null )
        {
            var tx = spawnpoint.Transform;
            tx.Position = tx.Position + Vector3.Up * 50.0f;
            Transform = tx;
        }
    }

    public override void ClientSpawn() { }

    protected void CreateHull()
    {
        SetupPhysicsFromOrientedCapsule(
           PhysicsMotionType.Keyframed,
           new Capsule(Vector3.Zero, Vector3.Up * 75, 16)
        );

        LifeState = LifeState.Alive;
        EnableAllCollisions = true;
    }

    protected void DestroyHull()
    {
        PhysicsClear();
        EnableAllCollisions = false;
    }

    public void Respawn()
    {
        EnableDrawing = true;
        Children.OfType<ModelEntity>().ToList().ForEach(x => x.EnableDrawing = true);

        SetupBodyController();
        CreateHull();

        //ClientRespawn( To.Single(Client) );

        clothing.DressEntity(this);
    }

    private void SetupBodyController()
    {
        Components.Create<PlayerBodyController>();
        Components.RemoveAny<PlayerBodyControllerMechanic>();
        Components.Create<WalkMechanic>();
        Components.Create<SprintMechanic>();
        Components.Create<SneakMechanic>();
        Components.Create<CrouchMechanic>();
        Components.Create<AirMoveMechanic>();
        Components.Create<JumpMechanic>();

        BodyController.Active = true;
    }

    private void DestroyComponents()
    {
        Components.RemoveAll();
    }

    [ConCmd.Admin("noclip")]
    private static void ToggleNoclip()
    {
        var player = ConsoleSystem.Caller.Pawn as Player;
        var noclip = player.Components.GetOrCreate<NoclipController>();
        if (player.ActiveController is NoclipController && player.BodyController != null)
        {
            player.BodyController.Active = true;
        }
        else
        {
            noclip.Active = true;
        }
    }

    [ClientRpc]
    public void ClientRespawn() { }

    /// <summary>
    /// Called every tick, clientside and serverside.
    /// </summary>
    public override void Simulate(IClient cl)
    {
        Rotation = LookInput.WithPitch(0f).ToRotation();

        if (Input.Pressed(InputButton.View))
        {
            ThirdPersonCamera = !ThirdPersonCamera;
        }

        TickPlayerUse();

        TickAnimation();

        ActiveController?.Simulate(cl);

        if (ActiveController is PlayerBodyController bodyController)
        {
            GroundEntity = bodyController.GroundEntity;
        }
        else
        {
            GroundEntity = null;
        }

        SimulateActiveChild(cl, ActiveChild);
    }

    /// <summary>
    /// Called every frame on the client
    /// </summary>
    public override void FrameSimulate(IClient cl)
    {
        Rotation = LookInput.WithPitch(0f).ToRotation();
        ActiveController?.FrameSimulate(cl);

        SimulateCamera(cl);
    }

    public override void OnKilled()
    {
        DestroyComponents();

        LifeState = LifeState.Dead;

        EnableDrawing = false;
        DestroyHull();
    }
}
