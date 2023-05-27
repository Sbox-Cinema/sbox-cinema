using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    private int MaxRollerItems = 22;
    private void TestHotDogs()
    {
        for (int i = 0; i < MaxRollerItems; i++)
        {
            Hotdogs.Add($"S{i + 1}F", new Hotdog());
            Hotdogs.Add($"S{i + 1}B", new Hotdog());
        }

        foreach (var hotdog in Hotdogs)
        {
            AttachEntity(hotdog.Key, hotdog.Value);
        }
    }

    private void AttachEntity(string attach, Entity ent)
    {
        if (GetAttachment(attach) is Transform t)
        {
            ent.Transform = t;

            ent.Position += (Vector3.Up * 0.5f);
            ent.Position += (Vector3.Left * 0.5f);
        }
    }
}
