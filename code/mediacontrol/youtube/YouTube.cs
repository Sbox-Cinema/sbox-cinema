using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public class YouTube
{
    /// <summary>
    /// This is a workaround to Github issue #41 (https://github.com/tech-nawar/sbox-cinema/issues/41), or it
    /// can be used to prevent clients from accessing cinema-api directly for whatever reason.
    /// </summary>
    [ConVar.Replicated("youtube.enableidvalidation")]
    public static bool ValidateYoutubeIds { get; set; } = true;

    public static string ParseVideoIdIfUrl(string userInput)
    {
        var possibleUrl = userInput;

        // If this is clearly a website but missing the URI scheme, add it here.
        if (userInput.StartsWith("youtu") || userInput.StartsWith("www.youtu"))
        {
            possibleUrl = "https://" + userInput;
        }

        // If we can't make URI from this input, just return the input as-is since it's probably an ID.
        if (!Uri.TryCreate(possibleUrl, UriKind.Absolute, out Uri uri))
        {
            return userInput;
        }
        if (uri.Scheme != Uri.UriSchemeHttps && uri.Scheme != Uri.UriSchemeHttp)
        {
            return userInput;
        }

        // Youtu.be doesn't use a query string and the video ID is just the last part of the path.
        if (uri.Host.Contains("youtu.be"))
        {
            return System.IO.Path.GetFileNameWithoutExtension(uri.LocalPath);
        }
        // If this is just some other website, return the input as-is.
        if (!uri.Host.Contains("youtube"))
        {
            return userInput;
        }

        var queryString = System.Web.HttpUtility.ParseQueryString(uri.Query);
        var videoIdParam = queryString["v"];
        if (string.IsNullOrEmpty(videoIdParam))
        {
            Log.Error("No video ID in URL");
            return null;
        }
        return videoIdParam;
    }

    public static async Task<bool> VerifyYouTubeId(string youTubeId)
    {
        if (!ValidateYoutubeIds)
            return true;

        ParseApiResponse response;

        try
        {
            response = await Http.RequestJsonAsync<ParseApiResponse>($"{CinemaApi.Url}/api/parse2?type=yt&id={youTubeId}", "GET");
        }
        catch (Exception e)
        {
            Log.Error(e.Message);
            return false;
        }

        return response.CanEmbed;
    }
}
