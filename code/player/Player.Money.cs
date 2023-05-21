using Sandbox;

namespace Cinema;

public partial class Player
{
    // The players current money
    [Net] public int Money { get; set; }

    /// <summary>
    /// Checks if a player can afford a certain amount of money.
    /// </summary>
    /// <param name="amount">How much money</param>
    /// <returns>True if money >= the amount</returns>
    public bool CanAfford(int amount)
    {
        return Money >= amount;
    }

    /// <summary>
    /// Adds money to the player's money.
    /// </summary>
    /// <param name="amount">The amount to add</param>
    public void AddMoney(int amount)
    {
        Money += amount;
    }

    /// <summary>
    /// Takes money from the player.
    /// </summary>
    /// <param name="amount">How much to take</param>
    public void TakeMoney(int amount)
    {
        Money -= amount;

        if (Money < 0)
            Money = 0;
    }

    /// <summary>
    /// Sets the amount of money the player has.
    /// </summary>
    /// <param name="amount">How much money to set</param>
    public void SetMoney(int amount)
    {
        Money = amount;
    }
}
