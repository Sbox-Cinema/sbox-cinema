using Sandbox;
using Sandbox.UI;
using Cinema.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class Player
{
    [ClientInput]
    public bool IsMenuOpen { get; set; }
    public Panel ActiveMenu 
    {
        get => _ActiveMenu;
        set
        {
            _ActiveMenu = value;
            IsMenuOpen = value != null;
        } 
    }
    private Panel _ActiveMenu;

    public void SimulateUI()
    {
        if (Input.Pressed("reload"))
        {
            if (ActiveMenu is null)
            {
                var closestProjector = Entity.All.OfType<ProjectorEntity>().OrderBy(x => x.Position.Distance(Game.LocalPawn.Position)).FirstOrDefault();
                if (closestProjector != null)
                {
                    var queue = MovieQueue.Instance;
                    queue.Controller = closestProjector.Controller;
                    queue.Visible = true;
                    ActiveMenu = queue;
                }
            }
            else if (ActiveMenu is MovieQueue queue)
            {
                queue.Visible = false;
                queue.Controller = null;
                ActiveMenu = null;
            }
        }
    }
}
