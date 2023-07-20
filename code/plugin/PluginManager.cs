using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public static class PluginManager
{
    /// <summary>
    /// If set to 0, nobody will be able to add plugins. <br/>
    /// (TODO) If set to 1, only authorized persons will be able to add plugins.<br/>
    /// If set to 2, everyone will be able to add plugins.<br/>
    /// </summary>
    [ConVar.Replicated("plugin.media.allowadd")]
    public static int AllowAdd { get; set; } = 2;

    [ConVar.Replicated("plugin.localonly")]
    public static bool UseLocalOnly { get; set; } = false;

    [ConCmd.Server("plugin.add")]
    public static async void AddPlugin(string ident)
    {
        if (AllowAdd <= 0)
        {
            Log.Info("Adding media plugins is disabled.");
            return;
        }
        Log.Info($"{ConsoleSystem.Caller} - Adding plugin {ident}");
        var package = await Package.FetchAsync(ident, false);
        Log.Info($"Fetched package: {package.FullIdent}");
        await package.MountAsync(true);
        VideoProviderManager.Instance.Refresh();
    }

    [ConCmd.Server("plugin.media.list.remote")]
    public static async void FindAvailablePluginsRemote()
    {
        var packages = await FindPlugins("sort:popular type:library +ctmi +plugin", false);
        Log.Info($"Found {packages.Count} matching plugins.");
        foreach(var package in packages)
        {
            Log.Info($"\t{package.FullIdent}");
        }
    }

    [ConCmd.Server("plugin.media.list.local")]
    public static async void FindAvailablePluginsLocal()
    {
        var packages = await FindPlugins("sort:popular local:true type:library +ctmi +plugin", false);
        packages = FilterPlugins(packages).ToList();
        Log.Info($"Found {packages.Count} matching plugins.");
        foreach (var package in packages)
        {
            Log.Info($"\t{package.FullIdent}");
        }
    }

    public static IEnumerable<Package> FilterPlugins(IEnumerable<Package> packages)
        => packages
            .Where(
                p => p.Tags.Contains("ctmi") && p.Tags.Contains("plugin")
            );

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
