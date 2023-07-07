using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CinemaTeam.Plugins.Video;

public partial class CuratedMediaHeaderPanel : MediaProviderHeaderPanel
{
    public void OnPressButton()
    {
        OnRequestMedia(new IMediaSelector.MediaRequestEventArgs
        {
            Query = null
        });
    }
}
