
using System.Threading.Tasks;
using Sandbox;

namespace Cinema;

public partial class CinemaGame
{

    // Precache assets we know we're going to need
    private static void MountAssets()
    {
        if (!Game.IsServer) return;

        _ = DownloadAsset("facepunch.metal_wheely_bin"); //models/sbox_props/metal_wheely_bin/metal_wheely_bin.vmdl
    }

    public static async Task DownloadAsset(string packageName)
    {
        var package = await Package.Fetch(packageName, false);
        if (package == null || package.PackageType != Package.Type.Model || package.Revision == null)
        {
            // spawn error particles
            return;
        }

        var model = package.GetMeta("PrimaryAsset", "models/dev/error.vmdl");
        // downloads if not downloaded, mounts if not mounted
        await package.MountAsync();

        Precache.Add(model);
    }
}
