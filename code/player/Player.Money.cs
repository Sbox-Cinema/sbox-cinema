using Sandbox;

namespace Cinema;

public partial class Player
{
    [Net] public int Money { get; set; }

    public bool CanAfford(int amount)
    {
        return Money >= amount;
    }

    public void AddMoney(int amount)
    {
        Money += amount;
    }

    public void TakeMoney(int amount)
    {
        Money -= amount;

        if (Money < 0)
            Money = 0;
    }

    public void SetMoney(int amount)
    {
        Money = amount;
    }
}
