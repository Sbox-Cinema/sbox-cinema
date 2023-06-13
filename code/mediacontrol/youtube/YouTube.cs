using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public partial class YouTube : IMediaProvider
{
    public string ProviderName => throw new NotImplementedException();

    public MediaProviderHeaderPanel HeaderPanel => throw new NotImplementedException();

    public event EventHandler<IMediaProvider.OnRequestMediaEventArgs> OnRequestMedia;

    public void Pause()
    {
        throw new NotImplementedException();
    }

    public void Play(string RequestData)
    {
        throw new NotImplementedException();
        var player = new VideoPlayer();
    }

    public void Seek(float Time)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }
}
