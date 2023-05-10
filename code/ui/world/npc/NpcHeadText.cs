using Sandbox;

namespace Cinema.UI;

public partial class NpcHeadText
{
    public NpcBase Npc { get; set; }
    private string Header => Npc?.Name ?? "";
    private string Description => Npc?.Description ?? "";

    public NpcHeadText(NpcBase npc)
    {
        Npc = npc;
    }

    public override void Tick()
    {
        if (!Npc.IsValid())
        {
            Delete();
            return;
        }

        Position = Npc.GetBoneTransform("head").Position + Vector3.Up * 4;
        Rotation = Rotation.LookAt(Camera.Rotation.Forward * -1.0f, Vector3.Up);
    }
}
