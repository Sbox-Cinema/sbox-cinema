using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public interface IMediaProvider
{
    string ProviderName { get; }
    bool HasVideo { get; }
    bool HasAudio { get; }
    Panel HeaderPanel { get; }
    float Volume { get; set; }
    void Play(IMediaPresentationContext context);
    void Pause();
    void Stop();
    void Seek(int timeInSeconds);
}
