using Cinema.Jobs;
using Sandbox;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cinema;

public static partial class CinemaEvent
{
    public const string JobChanged = "job.changed";

    [MethodArguments(typeof( Player ))]
    public class JobChangedAttribute : EventAttribute
    {
        public JobChangedAttribute() : base(JobChanged) { }
    }
}
