using Sandbox;
using Sandbox.Engine.Utility.RayTrace;
using System.Linq;

namespace Cinema;

public partial class GooglyEye : WeaponBase
{
    PreviewEntity PreviewModel { get; set; }
    public override void Spawn()
    {
        base.Spawn();

        PhysicsEnabled = true;
        UsePhysicsCollision = true;
        EnableHideInFirstPerson = true;
        EnableShadowInFirstPerson = true;

        ViewModelPath = "models/googly_eyes/v_googly_eyes_01.vmdl";
    }

    public override void ClientSpawn()
    {
        base.ClientSpawn();

        if (!PreviewModel.IsValid())
        {
            PreviewModel = new PreviewEntity
            {
                Predictable = true,
                Owner = Owner
            };

            PreviewModel.SetModel("models/googly_eyes/preview_googly_eyes_01.vmdl");
        }
    }

    protected virtual bool IsPreviewTraceValid(TraceResult tr)
    {
        if (!tr.Hit)
            return false;

        if (!tr.Entity.IsValid())
            return false;

        return true;
    }

    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
       
        if (Game.IsServer)
        {
            using (Prediction.Off())
            {
                if (!Input.Pressed("attack1"))
                    return;

                var ray = Owner.AimRay;
                var tr = Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * 1000.0f))
                .WithAllTags("solid")
                .Ignore(Owner)
                .Run();

                var ent = new GooglyEyeEntity
                {
                    Position = tr.EndPosition,
                    Rotation = Rotation.LookAt(tr.Normal, Owner.AimRay.Forward) * Rotation.From(new Angles(90, 0, 0)),
                    Model = Model.Load("models/googly_eyes/googly_eyes_01.vmdl")
                };
            }
        }
    }

    [GameEvent.Client.Frame]
    public void OnClientFrame()
    {
        var ray = Owner.AimRay;
        var tr = Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * 60.0f))
        .WithAllTags("solid")
        .Ignore(Owner)
        .Run();

        if (IsPreviewTraceValid(tr) && PreviewModel.UpdateFromTrace(tr))
        {
            PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(0.5f);
        }
        else
        {
            PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(0.0f);
        }
    }
}
