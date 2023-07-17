using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public static class PluginManager
{
    [ConCmd.Server("plugin.add")]
    public static async void AddPlugin(string ident)
    {
        Log.Info($"{ConsoleSystem.Caller} - Adding plugin {ident}");
        var package = await Package.FetchAsync(ident, false);
        await package.MountAsync(true);
        VideoProviderManager.Instance.Refresh();
    }

    [ConCmd.Server("plugins.video.available")]
    public static async void FindAvailablePlugins()
    {
        var queryString = $"+ctmi +plugin sort:popular type:library";
        var foundPackages = await FindPlugins(queryString, true);
        Log.Info($"Found {foundPackages.Count} plugins.");
        foreach ( var foundPackage in foundPackages )
        {
            Log.Info($"Found plugins: {foundPackage.FullIdent}");
        }
    }

    public static async Task<List<Package>> FindPlugins(string queryString, bool excludeInstalled = true)
    {
        var foundPackages = new List<Package>();
        var found = await Package.FindAsync(queryString);

        if (!excludeInstalled)
            return found.Packages.ToList();

        var wasInstalledAtStart = (Package p) => Game.Server.RequiredContent.Contains(p.FullIdent);
        var notInstalledPackages = found
            .Packages
            .Where(p =>
            {
                return !wasInstalledAtStart(p) && !p.IsMounted();
            })
            .ToList();
        
        return notInstalledPackages;
    }
}
