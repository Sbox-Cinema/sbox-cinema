using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class Hygiene : BaseNeed
{
    public override string DisplayName => "Hygiene";
    public override string IconPath => "shower";
    public override float SatisfactionPercent => 100f;
}
