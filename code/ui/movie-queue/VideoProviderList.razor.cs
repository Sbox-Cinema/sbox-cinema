using CinemaTeam.Plugins.Video;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Cinema.UI;

public partial class VideoProviderList : Panel
{
    public VideoProviderList()
    {
        RefreshVideoProviders();
    }

    private List<IMediaProvider> Providers { get; set; }
    public IMediaProvider SelectedProvider
    {
        get => _selectedProvider;
        set
        {
            if (_selectedProvider == value)
                return;

            _selectedProvider = value;
            // Find the panel that has this provider.
            var providerPanel = Children
                .OfType<VideoProviderIcon>()
                .FirstOrDefault(p => p.Provider == value);
            // Select the panel we found.
            providerPanel?.SetClass("selected", true);
            DeselectOtherPanels(value);
            OnProviderSelected?.Invoke(value);
        }
    }
    private IMediaProvider _selectedProvider;

    public Action<IMediaProvider> OnProviderSelected { get; set; }

    public void RefreshVideoProviders()
    {
        Providers = new List<IMediaProvider>(VideoProviderManager.Instance.GetAll());
    }

    public void OnClickedProvider(IMediaProvider provider)
    {
        SelectedProvider = SelectedProvider == provider
            ? null
            : provider;
    }

    private void DeselectOtherPanels(IMediaProvider keepSelected)
    {
        var otherProviderPanels = Children
            .OfType<VideoProviderIcon>()
            .Where(p => p.Provider != keepSelected);
        foreach (var panel in otherProviderPanels)
        {
            panel.SetClass("selected", false);
        }
    }

    protected override int BuildHash()
    {
        int hash = 0;
        foreach (var provider in Providers)
        {
            hash = HashCode.Combine(hash, provider);
        }
        return hash;
    }
}
