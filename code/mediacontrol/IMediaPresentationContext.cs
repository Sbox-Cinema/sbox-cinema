using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

/// <summary>
/// Provides access to resources that are used to present media in a theater.
/// </summary>
public interface IMediaPresentationContext
{
    Texture VideoOutput { get; }
    
}
