using Sandbox;

namespace Cinema;

public partial class GooglyEye : WeaponBase
{
    static private float PlacementDistance = 128.0f;

    PreviewEntity PreviewModel { get; set; }
    
    public override void Spawn()
    {
        base.Spawn();
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
        if (!tr.Hit || !tr.Entity.IsValid()) return false;

        if (tr.Entity is GooglyEyeEntity) return false;

        return true;
    }
    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);
       
        if(Game.IsServer)
        {
            using (Prediction.Off())
            {
                if (!Input.Pressed("attack1")) return;
               
                var ray = Owner.AimRay;
                var distance = PlacementDistance;
                var tr = Trace.Ray(ray, distance)
                    .WithoutTags("player", "weapon", "item", "clothes", "npc")
                    .Ignore(Owner)
                    .Run();

                if (IsPreviewTraceValid(tr))
                {
                    var ent = new GooglyEyeEntity
                    {
                        Parent = tr.Entity,
                        Position = tr.HitPosition,
                        Rotation = Rotation.LookAt(tr.Normal, Owner.AimRay.Forward) * Rotation.From(new Angles(90, 0, 0)),
                        Model = Model.Load("models/googly_eyes/googly_eyes_01.vmdl"),
                    };
                }
                
            }
        }
    }

    [GameEvent.Client.Frame]
    public void OnClientFrame()
    {
        var ray = Owner.AimRay;
        var tr = Trace.Ray(ray, PlacementDistance)
            .WithoutTags("player", "weapon", "item", "clothes", "npc")
            .Ignore(Owner)
            .Run();
       
        if((Game.LocalPawn as Player).ActiveChild is GooglyEye)
        {
            if (IsPreviewTraceValid(tr) && PreviewModel.UpdateFromTrace(tr))
            {
                PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(1.0f);
            }
            else
            {
                PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(0.4f);
                PreviewModel.Rotation = Rotation.LookAt(tr.Normal, Owner.AimRay.Forward) * Rotation.From(new Angles(90, 0, 0));
                PreviewModel.Position = ray.Project(PlacementDistance);
            }
        }
    }
}
