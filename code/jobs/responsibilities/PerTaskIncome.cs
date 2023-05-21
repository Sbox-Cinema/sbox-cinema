using Sandbox;

namespace Cinema.Jobs;

/// <summary>
/// A income that pays out for every task success
/// </summary>
public partial class PerTaskIncome : JobResponsibility
{
    public override string Name => "Per Task Income";

    //The income per task completion
    //This is fixed to this income, we need to think of something -ItsRifter
    private static int IncomeAmount => 10;

    protected override void OnActivate()
    {
        
    }

    /// <summary>
    /// Get the pay income
    /// </summary>
    /// <returns>The amount of income</returns>
    public static int GetIncomePay()
    {
        return IncomeAmount;
    }
}
