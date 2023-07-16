using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.UI;

public partial class RemotePluginBrowser : Panel
{
    public string VisibilityClass => IsOpen ? "visible" : "";
    /// <summary>
    /// The portion of a query string that is used to include or exclude tags.
    /// </summary>
    public string TagQuery { get; set; }
    
    private bool IsOpen { get; set; }
    private List<Package> FoundPackages { get; set; } = new();
    private string GetQueryString() => $"{TagQuery} sort:popular type:library";

    public async void Open()
    {
        await FindPlugins();
        StateHasChanged();
        IsOpen = true;
    }

    public void Close()
    {
        FoundPackages.Clear();
        IsOpen = false;
    }

    public async Task FindPlugins()
    {
        var found = await Package.FindAsync(GetQueryString());
        Log.Info($"{GetQueryString()}: Found {found.TotalCount} packages.");
        if (found != null)
        {
            FoundPackages.AddRange(found.Packages);
        }
    }
}
