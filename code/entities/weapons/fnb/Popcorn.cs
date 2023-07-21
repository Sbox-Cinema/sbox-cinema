using Conna.Inventory;
using Sandbox;
using Sandbox.util;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;


namespace Cinema;

public partial class Popcorn : WeaponBase
{
    public override float PrimaryFireRate => 0.85f;
    public override int BaseUses => 10;
    private string UniqueId = "popcorn_tub"; // The unique id for the hotdog item
    private Player Owner;
    public override void Spawn()
    {
        base.Spawn();
    }

    public override void PrimaryFire()
    {
        base.PrimaryFire();

        PlaySound("placeholder_eating");
    }

    public override void SecondaryFire()
    {
        if (Game.IsClient) return;

        using (Prediction.Off())
        {
            var projectile = new Projectile()
            {
                Model = Model.Load("models/popcorn_tub/w_popcorn_tub_01.vmdl"),
            };

            projectile.LaunchFromEntityViewpoint(WeaponHolder);
            
            if (Projectile.AutoRemoveThrown)
                RemoveFromHolder();
        }
    }

    public override void Reload()
    {
    }

    public override void Trigger (Player player)
    {
        base.OnUse(player);

        Owner = player;
    }
}
