using Sandbox;
using System.Collections.Generic;
using System.Linq;
using CinemaTeam.Plugins.Media;

namespace Cinema;

public class VideoProviderManager
{
    private IDictionary<int, TypeDescription> Providers { get; set; }

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
        Providers = new Dictionary<int, TypeDescription>();
        // Get all video providers from this game and the initially loaded addons.
        foreach (var provider in GetLoadedProviders())
        {
            var ident = TypeLibrary.GetTypeIdent(provider.TargetType);
            // Log the provider type ident and name.
            Log.Trace($"{ident}: {provider.Name}");
            Providers.Add(ident, provider);
        }
    }

    /// <summary>
    /// Returns a <c>TypeDescription</c> for each <c>IVideoProvider</c> currently loaded by 
    /// this game and whatever addons and libraries may be loaded.
    /// </summary>
    private static IEnumerable<TypeDescription> GetLoadedProviders()
        => TypeLibrary.GetTypes<IMediaProvider>()
            .Where(t => !t.IsAbstract && !t.IsInterface); // Concrete types only.

    public IMediaProvider this[int key]
    {
        get
        {
            if (!Providers.ContainsKey(key))
            {
                return null;
            }

            return Providers[key].Create<IMediaProvider>();
        }
    }

    [ConCmd.Client("plugins.video.dumpall")]
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

        foreach(var kvp in Instance.Providers)
        {
            Log.Info($"\t{kvp.Key} - {kvp.Value.ClassName}");
        }
    }

    /// <summary>
    /// Gets the key associated with the specified video provider, which is a 
    /// hash of the fully qualified name of the video provider's type.
    /// </summary>
    /// <param name="provider"></param>
    /// <returns></returns>
    public int GetKey(IMediaProvider provider)
        => Providers.FirstOrDefault(p => p.Value.TargetType == provider.GetType()).Key;

    public IEnumerable<IMediaProvider> GetAll()
        => Providers.Values.Select(t => t.Create<IMediaProvider>());
}
