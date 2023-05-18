using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    /// <summary>
    ///
    /// </summary>
    private void TogglePower()
    {
        IsOn = !IsOn;

        UpdateMachine();
    }
    /// <summary>
    ///
    /// </summary>
    private void UpdateMachine()
    {
        UpdatePowerIndicators();
        UpdatePowerSwitches();
        UpdateControlKnobs();
        UpdateRollers();
        UpdateHotdogs();
    }
    /// <summary>
    ///
    /// </summary>
    private void UpdatePowerIndicators()
    {
        if (IsOn)
        {
            SetMaterialGroup(1);
        }
        else
        {
            SetMaterialGroup(0);
        }
    }

    /// <summary>
    ///
    /// </summary>
    private void UpdatePowerSwitches()
    {
        SetAnimParameter("toggle_left", IsOn);
        SetAnimParameter("toggle_right", IsOn);
    }

    /// <summary>
    ///
    /// </summary>
    private void UpdateControlKnobs()
    {
        if (IsOn)
        {
            SetAnimParameter("LeftHandleState", 3);
            SetAnimParameter("RightHandleState", 3);
        }
        else
        {
            SetAnimParameter("LeftHandleState", 0);
            SetAnimParameter("RightHandleState", 0);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void UpdateRollers()
    {

    }

    /// <summary>
    ///
    /// </summary>
    private void UpdateHotdogs()
    {
        if (IsOn) 
        {
            foreach (var hotdog in HotdogsFront)
            {
                hotdog.Components.Create<Rotator>();
                hotdog.Components.Create<Cooking>();
            }

            foreach (var hotdog in HotdogsBack)
            {
                hotdog.Components.Create<Rotator>();
                hotdog.Components.Create<Cooking>();
            }
        } 
        else
        {
            foreach (var hotdog in HotdogsFront)
            {
                hotdog.Components.RemoveAll();
            }

            foreach (var hotdog in HotdogsBack)
            {
                hotdog.Components.RemoveAll();
            }
        }
    }
}
