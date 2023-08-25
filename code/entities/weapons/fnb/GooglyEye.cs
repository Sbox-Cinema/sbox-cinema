using Sandbox;

namespace Cinema;

public partial class GooglyEye : WeaponBase
{
    static private float MaxPlacementDistance = 96.0f;
    PreviewEntity PreviewModel { get; set; }

    protected virtual bool IsPreviewTraceValid(TraceResult tr)
    {
        if (!tr.Hit || !tr.Entity.IsValid() || tr.Entity is GooglyEyeEntity) return false;

        return true;
    }
    public override void Simulate(IClient cl)
    {
        base.Simulate(cl);

        if (Game.IsClient) return;
        
        using (Prediction.Off())
        {
            if (!Input.Pressed("attack1")) return;
               
            var ray = Owner.AimRay;
            var distance = MaxPlacementDistance;
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
                    Rotation = Rotation.LookAt(tr.Normal, Owner.AimRay.Forward) * Rotation.From(new Angles(90, 0, 0))
                };
            }
        }
    }

    public override void ActiveStart(Entity ent)
    {
        base.ActiveStart(ent);

        if (Game.IsServer) return;
        
        PreviewModel = PreviewModel.IsValid() ? PreviewModel : new PreviewEntity
        {
            Model = Model.Load("models/googly_eyes/preview_googly_eyes_01.vmdl")
        };
    }

    public override void ActiveEnd(Entity ent, bool dropped)
    {
        base.ActiveEnd(ent, dropped);

        if (Game.IsServer) return;

        PreviewModel?.Delete();
    }

    [GameEvent.Tick.Client]
    public void OnClientTick()
    {
        if (!Owner.IsAuthority || (Game.LocalPawn as Player).ActiveChild is not GooglyEye) return;

        var ray = Owner.AimRay;
        var tr = Trace.Ray(ray, MaxPlacementDistance)
            .WithoutTags("player", "weapon", "item", "clothes", "npc")
            .Ignore(Owner)
            .Run();

            
        if (IsPreviewTraceValid(tr) && PreviewModel.UpdateFromTrace(tr))
        {
            PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(1.0f);
        }
        else
        {
            PreviewModel.RenderColor = PreviewModel.RenderColor.WithAlpha(0.4f);
            PreviewModel.Rotation = Rotation.LookAt(tr.Normal, Owner.AimRay.Forward) * Rotation.From(new Angles(90, 0, 0));
            PreviewModel.Position = ray.Project(MaxPlacementDistance);
        }
    }
}
