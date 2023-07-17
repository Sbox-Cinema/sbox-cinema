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
    public Action<Package> OnPackageSelected { get; set; }
    
    private bool IsOpen { get; set; }
    private Panel PackageList { get; set; }
    private List<Package> FoundPackages { get; set; } = new();
    private string GetQueryString() => $"{TagQuery} sort:popular type:library";


    public async void Open()
    {
        var queryString = GetQueryString();
        FoundPackages = await PluginManager.FindPlugins(queryString);
        Log.Info($"{queryString}: Found {FoundPackages.Count} packages not yet installed.");
        StateHasChanged();
        IsOpen = true;
    }

    public void Close()
    {
        FoundPackages.Clear();
        IsOpen = false;
    }

    public void SelectPackage(Package package)
    {
        Log.Info("Selected package: " + package.FullIdent);
        // Delete the package icon.
        FoundPackages.Remove(package);
        StateHasChanged();
        PluginManager.AddPlugin(package.FullIdent);
        OnPackageSelected?.Invoke(package);
    }
}
