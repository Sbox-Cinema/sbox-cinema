using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema;

partial class Player : AnimatedEntity, IEyes
{
    public ClothingContainer Clothing { get; protected set; }

    [BindComponent]
    public PlayerBodyController BodyController { get; }

    public IEnumerable<PlayerController> PlayerControllers => Components.GetAll<PlayerController>();

    public PlayerController ActiveController => PlayerControllers.FirstOrDefault((e) => e.Active);

    public static Model PlayerModel = Model.Load("models/citizen/citizen.vmdl");

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

        Tags.Add("player");
    }

    public void LoadClientClothingSettings(IClient cl)
    {
        Clothing ??= new();
        Clothing.LoadFromClient(cl);
    }

    public override void ClientSpawn() { }

    public void Respawn()
    {

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

        SetupPhysicsFromAABB(
            PhysicsMotionType.Keyframed,
            new Vector3(-16, -16, 0),
            new Vector3(16, 16, 72)
        );

        EnableAllCollisions = true;
        EnableDrawing = true;
        Children.OfType<ModelEntity>().ToList().ForEach(x => x.EnableDrawing = true);

        SetupBodyController();
        BodyController.Active = true;

        ClientRespawn(To.Single(Client));
        if (Clothing == null)
        {
            LoadClientClothingSettings(Client);
        }
        Clothing.DressEntity(this);
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
}
