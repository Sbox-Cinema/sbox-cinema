using Sandbox;
using System.Linq;

namespace Cinema;

public partial class Player
{
    [Net]
    public string ClothingAsString { get; set; }

    /// <summary>
    /// The clothing that the player chosen for themself in the avatar editor.
    /// </summary>
    public ClothingContainer AvatarClothing { get; protected set; }

    /// <summary>
    /// Dresses the player entity.
    /// It will load the player's clothing from the client data if it hasn't been loaded yet.
    /// </summary>
    public void LoadAvatarClothing()
    {
        if (Client == null || !Client.IsValid) return;

        Undress();

        if (AvatarClothing == null)
        {
            AvatarClothing ??= new();
            AvatarClothing.LoadFromClient(Client);
            ClothingAsString = Client.GetClientData("avatar", "");
        }

        AvatarClothing.DressEntity(this);
    }

    /// <summary>
    /// Deletes all clothing currently worn by a player and makes all body groups visible.
    /// </summary>
    public void Undress()
    {
        var clothing = Children.Where(e => e.Tags.Has("clothes")).ToList();
        for (int i = 0; i < clothing.Count; i++)
        {
            clothing[i].Delete();
        }
        var bodyPartCount = Model.BodyPartCount;
        for (int i = 0; i < bodyPartCount; i++)
        {
            SetBodyGroup(i, 1);
        }
    }

    public void SetDrawTaggedClothing(string tag, bool enableDrawing)
    {
        var clothing = Children.Where(e => e.Tags.Has("clothes") && e.Tags.Has(tag)).ToList();
        for (int i = 0; i < clothing.Count; i++)
        {
            clothing[i].EnableDrawing = enableDrawing;
        }
    }

    public void DrawHead(bool enableDrawing)
    {
        SetBodyGroup(0, enableDrawing ? 0 : 1);
    }

}
