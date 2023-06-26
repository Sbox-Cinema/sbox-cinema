using Sandbox;
using System.Collections.Generic;
using System.Linq;
using CinemaTeam.Plugins.Video;

namespace Cinema;

public class VideoProviderManager
{
    private IDictionary<int, IVideoProvider> Providers { get; set; }

    public static VideoProviderManager Instance 
    { 
        get => _Instance ??= new VideoProviderManager();
        private set => _Instance = value;
    }
    private static VideoProviderManager _Instance;

    public VideoProviderManager()
    {
        Instance = this;
        Initialize();
    }

    public void Initialize()
    {
        // Get all video providers from this game and the initially loaded addons.
        Providers = GetLoadedProviders()
            .ToDictionary(t => TypeLibrary.GetTypeIdent(t.GetType()));
    }

    /// <summary>
    /// Returns all video providers that are currently loaded by this game and
    /// whatever addons and libraries may be loaded.
    /// </summary>
    /// <returns>A collection of all currently loaded video providers.</returns>
    private static IEnumerable<IVideoProvider> GetLoadedProviders()
        => TypeLibrary.GetTypes<IVideoProvider>()
            .Where(t => !t.IsAbstract && !t.IsInterface) // Concrete types only.
            .Select(t => TypeLibrary.Create<IVideoProvider>(t.Name));

    public IVideoProvider this[int key]
    {
        get
        {
            if (!Providers.ContainsKey(key))
            {
                return null;
            }

            return Providers[key];
        }
    }

    [ConCmd.Server("plugins.video.dumpall")]
    public static void DumpVideoProviders()
    {
        if (!Instance.Providers.Any())
        {
            Log.Info("No video providers were loaded.");
            return;
        }
        else
        {
            Log.Info($"Loaded {Instance.Providers.Count} video providers:");
        }

        foreach(var provider in Instance.Providers.Values)
        {
            Log.Info($"\t{provider.ProviderName}");
        }
    }

    /// <summary>
    /// Gets the key associated with the specified video provider, which is a 
    /// hash of the fully qualified name of the video provider's type.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public int GetKey(IVideoProvider provider)
        => Providers.FirstOrDefault(p => p.Value == provider).Key;

    public IEnumerable<IVideoProvider> GetAll()
        => Providers.Values;
}
