using Sandbox;
using Sandbox.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public interface IVideoProvider
{
    public string ProviderName { get; }
    public Panel SelectorPanel { get; }
    public void PlayVideo(string )
}
