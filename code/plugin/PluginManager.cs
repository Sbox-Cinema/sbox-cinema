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
    
}
