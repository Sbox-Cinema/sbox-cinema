using CinemaTeam.Plugins.Video;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class ProjectorEntity
{
    public static ProjectorEntity FindNearestToClient()
    {
        if (Game.LocalPawn is not Player ply)
            return null;
        
        return Entity
            .All
            .OfType<ProjectorEntity>()
            .OrderBy(proj => proj.Position.Distance(ply.Position))
            .FirstOrDefault();
    }

    [ConCmd.Client("projector.video.test.dog")]
    public static void PlayDogTestVideo()
    {
        var nearestProjector = FindNearestToClient();

        if (nearestProjector == null)
        {
            Log.Info($"Could not find nearest projector.");
            return;
        }

        var testMedia = new DummyVideoPresenter();
        nearestProjector.SetMedia(testMedia);
        nearestProjector.PlayOverheadAudio();
    }

    [ConCmd.Client("projector.video.test.youtube")]
    public async static void PlayYouTubeTestVideo()
    {
        var nearestProjector = FindNearestToClient();

        if (nearestProjector == null)
        {
            Log.Info($"Could not find nearest projector.");
            return;
        } 

        var youTubeProviderType = TypeLibrary
            .GetTypes<IVideoProvider>()
            .FirstOrDefault(t => t.ClassName == "YouTubeVideoProvider");
        var youTubeProvider = youTubeProviderType.Create<IVideoProvider>();
        var youTubeCurator = youTubeProvider as IMediaCurator;
        var youTubeRequest = await youTubeCurator.GetRequest();
        var testMedia = await youTubeProvider.Play(youTubeRequest);

        nearestProjector.SetMedia(testMedia);
        nearestProjector.PlayOverheadAudio();
    }
}
