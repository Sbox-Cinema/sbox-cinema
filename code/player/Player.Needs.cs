using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class Player
{
    public Nutrition Hunger { get; set; }
    public Hydration Thirst { get; set; }
    public Urination NumberOne { get; set; }
    public Defecation NumberTwo { get; set; }
    public Hygiene Hygiene { get; set; }

    public void SetupNeeds()
    {
        Hunger = Components.Create<Nutrition>();
        Thirst = Components.Create<Hydration>();
        NumberTwo = Components.Create<Defecation>();
        NumberOne = Components.Create<Urination>();
        Hygiene = Components.Create<Hygiene>();

        Components.Create<Stomach>();
    }
}
