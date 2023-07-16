﻿using System.Threading.Tasks;
using Cinema;

namespace CinemaTeam.Plugins.Media;

public static class MediaRequestExtensions
{
    public async static Task<IMediaPlayer> GetPlayer(this MediaRequest request)
    {
        // Log request URL and providre ID
        Log.Info($"Request URL: {request["Url"]}");
        Log.Info($"Request Provider ID: {request.VideoProviderId}");
        var provider = VideoProviderManager.Instance[request.VideoProviderId];
        return await provider.Play(request);
    }
}
