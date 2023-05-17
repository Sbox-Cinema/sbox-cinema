﻿using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    private void TogglePower()
    {
        IsOn = !IsOn;

        UpdatePowerIndicator();
        UpdateCookingComponents();
        UpdateControlKnobs();
    }

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
                hotdog.Components.RemoveAny<Rotator>();
                hotdog.Components.RemoveAny<Steam>();
            }

            foreach (var hotdog in HotdogsBack)
            {
                hotdog.Components.RemoveAny<Rotator>();
                hotdog.Components.RemoveAny<Steam>();
            }
        }
    }

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
