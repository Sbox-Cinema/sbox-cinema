using Sandbox;
using System;

namespace Cinema;

public class Hydration : BaseNeed, ISingletonComponent
{
    public override string DisplayName => "Thirst";
    public override string IconPath => "local_drink";
}
