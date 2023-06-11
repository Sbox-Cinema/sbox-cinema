using Sandbox;
using System;

namespace Cinema;

public class Nutrition : BaseNeed, ISingletonComponent
{
    public override string DisplayName => "Hunger";
    public override string IconPath => "lunch_dining";
}
