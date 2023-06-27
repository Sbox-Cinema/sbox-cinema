using Sandbox;
using Cinema.UI;

namespace Cinema;

public partial class Player
{
    /// <summary>
    /// On the server and client, this will be true if the client has a menu open. 
    /// </summary>
    [ClientInput]
    public bool IsMenuOpen { get; protected set; }

    /// <summary>
    /// On the server and client, this will be the name of the currently open menu, or <c>string.Empty</c> 
    /// if no menu is open.
    /// </summary>
    [ClientInput]
    public string ActiveMenuName { get; protected set; }
    
    /// <summary>
    /// Contains what the user has typed in the chatbox, but has not sent it just yet.
    /// </summary>
    [ClientInput] 
    public string CurrentlyTyping { get; set; }

    // Players' world panels
    public NameTag HeadTag { get; set; }

    /// <summary>
    /// On the client, this is the active menu. On the server this will be
    /// null, but you can use <c>IsMenuOpen</c> and <c>ActiveMenuName</c> to
    /// get some information about the client's menu.
    /// </summary>
    protected IMenuScreen ActiveMenu
    {
        get => _ActiveMenu;
        set
        {
            _ActiveMenu = value;
            IsMenuOpen = value != null;
            // This must be an empty string rather than null, otherwise an exception is thrown by S&box
            // when it tries to network the value.
            ActiveMenuName = value?.Name ?? "";
        }
    }
    private IMenuScreen _ActiveMenu;

    /// <summary>
    /// Client only. Opens the specified menu for this player, closing any other open menu.
    /// </summary>
    public void OpenMenu(IMenuScreen menu)
    {
        if (menu == null)
            return;

        if (ActiveMenu != null)
            CloseMenu();

        ActiveMenu = menu.Open() ? menu : null;
    }

    /// <summary>
    /// Client only. Closes the the currently open menu, or if <c>specificMenu</c> is set,
    /// only closes that menu.
    /// </summary>
    public void CloseMenu(IMenuScreen specificMenu = null)
    {
        if (specificMenu != null && specificMenu != ActiveMenu)
        {
            return;
        }

        ActiveMenu?.Close();
        ActiveMenu = null;
    }

    /// <summary>
    /// Toggles the specified menu between open and closed.
    /// </summary>
    public void ToggleMenu(IMenuScreen menu)
    {
        if (ActiveMenu == menu)
        {
            CloseMenu(menu);
        }
        else
        {
            OpenMenu(menu);
        }
    }
    /// <summary>
    /// Initializes things like player name tags. Anything that is attached to the player.
    /// </summary>
    public void InitializeWorldPanels()
    {
        HeadTag = new NameTag(this);
    }

    public void SimulateUI()
    {
        if (Input.Pressed("reload") && ActiveMenu is null or MovieQueue)
        {
            ToggleMenu(MovieQueue.Instance);
        }
        
        if (Input.Pressed("jobmenu") && ActiveMenu is null or JobMenu)
        {
            ToggleMenu(JobMenu.Instance);
        }

        if (Input.Pressed("Score") && ActiveMenu is null or Scoreboard)
        {
            ToggleMenu(Scoreboard.Instance);
        }

        if (Input.Pressed("Chat") && ActiveMenu is null or Chat)
        {
            ToggleMenu(Chat.Instance);
        }
    }

    private void CleanupUI()
    {
        // The nametag is a worldpanel, so it must be cleaned up manually.
        HeadTag.Delete(true);
    }
}
