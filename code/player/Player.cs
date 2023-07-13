using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

partial class Player : AnimatedEntity, IEyes
{

    [BindComponent]
    public PlayerBodyController BodyController { get; }

    [BindComponent]
    public Footstepper Footstepper { get; }

    public IEnumerable<PlayerController> PlayerControllers => Components.GetAll<PlayerController>();

    public PlayerController ActiveController => PlayerControllers.FirstOrDefault((e) => e.Active);

    public static Model PlayerModel = Model.Load("models/citizen/citizen.vmdl");

    [Net, Predicted]
    public bool ThirdPersonCamera { get; set; }


    // How long this player has been on the server
    [Net]
    public TimeSince TimeSinceJoinedServer { get; set; }

    public Player()
    {
        if (Game.IsServer)
        {
            SetupInventory();
        }
    }

    public void MakePawnOf(IClient client) {
        client.Pawn = this;
        Inventory.AddConnection(client);
    }

    /// <summary>
    /// Called when the entity is first created
    /// </summary>
    public override void Spawn()
    {
        Model = PlayerModel;
        Predictable = true;
        // Default properties
        EnableDrawing = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;
        EnableLagCompensation = true;
        EnableHitboxes = true;
        TimeSinceJoinedServer = 0;

        SetJob(Jobs.JobDetails.DefaultJob);

        Tags.Add("player");
    }

    public override void ClientSpawn() 
    {
        InitializeWorldPanels();
    }

    public void Respawn()
    {
        SetupPhysicsFromOrientedCapsule(
            PhysicsMotionType.Keyframed,
            new Capsule(Vector3.Zero, Vector3.Up * 75, 16)
        );

        var spawnpoints = Entity.All.OfType<SpawnPoint>();

        // chose a random one
        var randomSpawnPoint = spawnpoints.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

        // if it exists, place the pawn there
        if (randomSpawnPoint != null)
        {
            var tx = randomSpawnPoint.Transform;
            tx.Position = tx.Position + Vector3.Up * 50.0f; // raise it up
            Transform = tx;
        }

        EnableAllCollisions = true;
        EnableDrawing = true;
        Children.OfType<ModelEntity>().ToList().ForEach(x => x.EnableDrawing = true);

        Components.Create<Footstepper>();

        SetupBodyController();
        BodyController.Active = true;

        LoadAvatarClothing();

        ClientRespawn(To.Single(Client));
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
        Components.Create<AntiStuckMechanic>();
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
        if (Input.Pressed("view"))
        {
            ThirdPersonCamera = !ThirdPersonCamera;
        }

        if (Input.Pressed("swap_shoulder"))
        {
            ThirdPersonSwapShoulder();
        }

        TickPlayerUse();

        ActiveController?.Simulate(cl);

        if (ActiveController is PlayerBodyController bodyController)
        {
            GroundEntity = bodyController.GroundEntity;
        }
        else
        {
            GroundEntity = null;
        }

        SimulateActiveChild(cl);
    }

    /// <summary>
    /// Called every frame on the client
    /// </summary>
    public override void FrameSimulate(IClient cl)
    {
        ActiveController?.FrameSimulate(cl);

        SimulateUI();
    }

    public override void OnAnimEventFootstep(Vector3 position, int foot, float volume)
    {
        Footstepper.DoFootstep(position, foot, volume);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Game.IsServer)
            return;

        CleanupUI();
    }

}
