using Cinema.Jobs;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

/// <summary>
/// Holds events specific to the Cinema game.
/// </summary>
public static partial class CinemaEvent
{
    public const string JobChanged = "job.changed";

    /// <summary>
    /// Runs on the client and server whenever a player's job changes. The
    /// only argument is the <c>Player</c> whose job had changed.
    /// </summary>
    [MethodArguments(typeof( Player ))]
    public class JobChangedAttribute : EventAttribute
    {
        public JobChangedAttribute() : base(JobChanged) { }
    }
}
