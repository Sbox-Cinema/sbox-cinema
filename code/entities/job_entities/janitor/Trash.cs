using Sandbox;
using Sandbox.Component;
using System;
using System.Linq;

namespace Cinema;

public partial class Trash : Prop, IUse
{
    public enum DecayEnum
    {
        Clean,
        Spoiled,
        Infested,
        Rotten,
    }

    [Net] DecayEnum decayStatus { get; set; } = DecayEnum.Clean;

    [ConVar.Server("cinema.itemdecay.interval")]
    public static double DecayInterval { get; set; } = 45.0;

    TimeSince decayTime;
    Glow glow;
    bool canUpdate;

    Particles flies;

    int basePayment => 1;
    int curPayment;
    int basePayReduct => 2;

    public override void Spawn()
    {
        Tags.Add("solid, junk");

        SetupPhysicsFromModel(PhysicsMotionType.Static);
    }

    //Sets up the trash entity where the projectile landed
    public void SetUp(Vector3 pos)
    {
        //Make sure its set on the ground
        var tr = Trace.Ray(pos, pos + Vector3.Down * 32.0f).WorldOnly().Run();

        //Move it up a bit so its not in the ground too much
        //And rotate it randomly
        Position = tr.EndPosition + Vector3.Up * 3;
        Rotation = Rotation.FromYaw(Game.Random.Float(-90f, 180f)) + Rotation.FromPitch(90.0f);

        //Get all players who can pick up garbage
        var janitors = Game.Clients.Where(x => x.Pawn is Player pawn &&
        pawn.Job.JobDetails.Abilities == Jobs.JobAbilities.PickupGarbage);

        //Set up decay timer and current payment
        decayTime = 0.01f;
        curPayment = basePayment;

        //Create the glow highlight effect
        CreateGlow(To.Multiple(janitors));

        //Can we break the object into its gibs but not delete it? -ItsRifter
        //Breakables.Break(this);
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        if (Game.IsServer)
        {
            //Get all garbage collectors and destroy the glow
            var janitors = Game.Clients.Where(x => x.Pawn is Player pawn &&
            pawn.Job.JobDetails.Abilities == Jobs.JobAbilities.PickupGarbage);

            DestroyGlow(To.Multiple(janitors));

            //Cleanup particles
            flies?.Destroy(true);
        }
    }

    [GameEvent.Tick.Server]
    protected void ServerTick()
    {
        //if its been a second since a decay update, reset the bool for the next
        if (Math.Round(decayTime) % DecayInterval == 1.0f && !canUpdate)
            canUpdate = true;

        //If its been the decay interval and isn't rotten
        if (Math.Round(decayTime) % DecayInterval == 0 && decayStatus != DecayEnum.Rotten)
        {
            //Can't update, probably because its been updated in a previous server tick
            if (!canUpdate) return;

            //Update the decay state
            UpdateDecay();
        }
    }

    [GameEvent.Client.Frame]
    protected void ClientFrame()
    {
        //Updates glow on client
        UpdateGlow();
    }

    public void Pickup(Player user)
    {
        //TODO: Taking the trash out + checks if they can carry
        Delete();
    }

    public void UpdateDecay()
    {
        decayStatus += 1;
        canUpdate = false;

        //Deducts payment rather than divide it
        int reduction = curPayment / basePayReduct;
        curPayment -= reduction;

        //The decaying stages
        if (decayStatus == DecayEnum.Spoiled)
        {
            RenderColor = new Color(0.85f, 0.85f, 0.85f);
        }
        else if (decayStatus == DecayEnum.Infested)
        {
            RenderColor = new Color(0.50f, 0.50f, 0.50f);
            flies = Particles.Create("particles/flies/fly_swarm.vpcf", this);
        }
        else if (decayStatus == DecayEnum.Rotten)
        {
            RenderColor = new Color(0.30f, 0.30f, 0.30f);
        }
    }

    [ClientRpc]
    void CreateGlow()
    {
        glow = Components.GetOrCreate<Glow>();
        glow.Color = Color.Green;
        //Make the glow not appear through the world
        glow.ObscuredColor = Color.Transparent;
        glow.Width = 0.25f;
    }

    void UpdateGlow()
    {
        var player = Game.LocalPawn as Player;

        //If there is no glow component
        if (glow == null)
        {
            //and the player can pick up garbage, create the glow
            if (player.Job.JobDetails.Abilities == Jobs.JobAbilities.PickupGarbage)
                CreateGlow();
            //If they can't, stop here
            else return;
        }
        //If there is a glow component
        else
        {
            //The players job changed, remove the glow component
            if (player.Job.JobDetails.Abilities != Jobs.JobAbilities.PickupGarbage)
                DestroyGlow();
        }

        //Enable the glow if the player is within range else disable it
        glow.Enabled = player.Position.Distance(Position) < 236.0f;

        //Update glow color based on decay state
        switch (decayStatus)
        {
            case DecayEnum.Spoiled: glow.Color = Color.Yellow; break;
            case DecayEnum.Infested: glow.Color = Color.Orange; break;
            case DecayEnum.Rotten: glow.Color = Color.Red; break;
        }
    }

    [ClientRpc]
    void DestroyGlow()
    {
        Components.RemoveAny<Glow>();
        glow = null;
    }

    private async void DisableGibPhysics()
    {
        await Task.DelaySeconds(1.0f);
    }

    public bool OnUse(Entity user)
    {
        if (!IsUsable(user)) return false;

        Pickup(user as Player);
        return false;
    }

    public bool IsUsable(Entity user)
    {
        if (user is Player player && player.Job.Abilities == Jobs.JobAbilities.PickupGarbage)
            return true;

        return false;
    }
}
