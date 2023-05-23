using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.UI;

public partial class PlayerInventory
{
    public static PlayerInventory Instance { get; private set; }

    public PlayerInventory()
    {
        Instance = this;
    }
}
