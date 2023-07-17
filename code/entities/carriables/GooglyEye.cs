using Sandbox;
using Sandbox.Engine.Utility.RayTrace;

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
                Owner = Owner,
                Model = Model.Load("models/googly_eyes/preview_googly_eyes_01.vmdl")
            };
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

    protected virtual bool IsPreviewTraceValid(MeshTraceRequest.Result tr)
    {
        if (!tr.Hit)
            return false;

        if (!tr.SceneObject.IsValid())
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
                var distance = 128.0f;
                var tr = Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * distance))
                .WithoutTags("player", "weapon", "item", "clothes", "npc")
                .Ignore(Owner)
                .Run();
                
                /*
                var ray = Owner.AimRay;
                var distance = 128.0f;
                var tr = Game.SceneWorld.Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * distance))
                .WithoutTags("player", "weapon", "item", "clothes", "npc")
                .Run();
                */

                if (IsPreviewTraceValid(tr))
                {
                    var ent = new ModelEntity
                    {
                        Position = tr.HitPosition,
                        Rotation = Rotation.LookAt(tr.Normal, Owner.AimRay.Forward) * Rotation.From(new Angles(90, 0, 0)),
                        Model = Model.Load("models/googly_eyes/googly_eyes_01.vmdl")
                    };
                }
            }
        }
    }

    [GameEvent.Client.Frame]
    public void OnClientFrame()
    {
        if ((Game.LocalPawn as Player).ActiveChild is not GooglyEye) return; 
        
        var ray = Owner.AimRay;
        var distance = 128.0f;
        var tr = Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * distance))
            .WithoutTags("player", "weapon", "item", "clothes", "npc")
            .Ignore(Owner)
            .Run();
        
        /*
        var ray = Owner.AimRay;
        var distance = 128.0f;
        var tr = Game.SceneWorld.Trace.Ray(ray.Position, ray.Position + (ray.Forward.Normal * distance))
        .WithoutTags("player", "weapon", "item", "clothes", "npc")
        .Run();
        */

        if (IsPreviewTraceValid(tr) && PreviewModel.UpdateFromTrace(tr))
        {
            PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(1.0f);
        }
        else
        {
            PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(0.25f);
            PreviewModel.Rotation = Rotation.LookAt(tr.Normal, Owner.AimRay.Forward) * Rotation.From(new Angles(90, 0, 0));
            PreviewModel.Position = ray.Position + (ray.Forward.Normal * distance);
        }   
    }
}
