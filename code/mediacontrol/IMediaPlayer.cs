using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema.mediacontrol
{
    public interface IMediaPlayer
    {
        bool HasVideo { get; }
        bool HasAudio { get; }
        Texture VideoTexture { get; }
        float Volume { get; set; }
        void Seek(int timeInSeconds);
        void Play();
        void Pause();
        void Stop();
    }
}
