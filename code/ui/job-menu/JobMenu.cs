using System.Linq;
using System;
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

namespace Cinema.UI;

public partial class JobMenu : Panel, IMenuScreen
{
    public class JobRow
    {
        public Jobs.JobDetails Job;
        public string Name => Job.Name;
        public string Description => Job.Description;
        public string LongDescription => Job.LongDescription;
    }
    protected static IEnumerable<JobRow> AvailableJobs => Jobs.JobDetails.All.Select(e => new JobRow
    {
        Job = e
    });

    public static string CurrentJobName => (Game.LocalPawn as Player).Job?.Name ?? "None";

    public static JobMenu Instance { get; set; }

    public bool IsOpen { get; protected set; }

    protected string VisibleClass => IsOpen ? "visible" : "";

    public string Name => "Job Menu";

    public JobRow SelectedJob { get; set; }

    public JobMenu()
    {
        Instance = this;
    }

    protected override int BuildHash()
    {
        return HashCode.Combine(IsOpen, SelectedJob);
    }

    public bool Open()
    {
        IsOpen = true;

        return true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public void SelectJob(JobRow job)
    {
        if (SelectedJob?.Name == job.Name)
        {
            SelectedJob = null;
            return;
        }

        SelectedJob = job;
    }

    protected static void BecomeJob(JobRow job)
    {
        Player.ClientRequestJobChange(job.Name);
    }

    protected void OnCloseClicked()
    {
        (Game.LocalPawn as Player).CloseMenu(this);
    }
}
