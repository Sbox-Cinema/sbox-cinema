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

    public void Undress()
    {
        var clothing = Children.Where(e => e.Tags.Has("clothes")).ToList();
        for (int i = 0; i < clothing.Count; i++)
        {
            clothing[i].Delete();
        }
    }

}
