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
        UpdatePowerIndicator();
        UpdatePowerButtons();
        UpdateCookingComponents();
        UpdateControlKnobs();
    }
    /// <summary>
    ///
    /// </summary>
    private void UpdatePowerIndicator()
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
    private void UpdatePowerButtons()
    {
        if (IsOn)
        {
            SetAnimParameter("toggle_left", true);
            SetAnimParameter("toggle_right", true);
        }
        else
        {
            SetAnimParameter("toggle_left", false);
            SetAnimParameter("toggle_right", false);
        }
    }
    /// <summary>
    ///
    /// </summary>
    private void UpdateCookingComponents()
    {
        if (IsOn) 
        {
            foreach (var hotdog in HotdogsFront)
            {
                hotdog.Components.Create<Rotator>();
                hotdog.Components.Create<Steam>();
            }

            foreach (var hotdog in HotdogsBack)
            {
                hotdog.Components.Create<Rotator>();
                hotdog.Components.Create<Steam>();
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
    /// <summary>
    ///
    /// </summary>
    private void UpdateControlKnobs()
    {
        Log.Info($"Updating control knobs { IsOn }");
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
}
