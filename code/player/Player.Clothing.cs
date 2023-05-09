using Sandbox;

namespace Cinema;

public partial class Player
{
    [Net]
    public string ClothingAsString { get; set; }

    public ClothingContainer Clothing { get; protected set; }

    public void LoadClothing()
    {
        if (Client == null || !Client.IsValid) return;

        if (Clothing == null)
        {
            Clothing ??= new();
            Clothing.LoadFromClient(Client);
            ClothingAsString = Client.GetClientData("avatar", "");
        }

        Clothing.DressEntity(this);
    }

}
