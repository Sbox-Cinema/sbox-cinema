using Cinema.Jobs;
using Sandbox;
using System;

namespace Cinema;

public partial class Player
{
    [BindComponent]
    public Jobs.PlayerJob Job { get; }

    //The default cooldown time for setting the forgiveness timer
    float JobCooldownDuration => 30.0f;

    /// <summary>
    /// The cooldown after leaving a job to prevent being assigned to a new job too quickly
    /// </summary>
    public TimeUntil TimeUntilStartNewJob { get; set; }

    /// <summary>
    /// Set the job to the player
    /// </summary>
    /// <param name="newJob">The details for that job to be assigned for the player</param>
    public void SetJob(Jobs.JobDetails newJob)
    {
        if (Game.IsClient) throw new System.Exception("Cannot set job on client!");

        UpdateResponsibilities(newJob.Responsibilities);

        Components.RemoveAny<Jobs.PlayerJob>();
        Components.Add(Jobs.PlayerJob.CreateFromDetails(newJob));
    }

    /// <summary>
    /// Updates the responsibility components on the player for a new job.
    /// Will remove any responsibilities that are not in the new job, and add any that are.
    /// </summary>
    /// <param name="newJob">New job responsibilities</param>
    private void UpdateResponsibilities(Jobs.JobResponsibilities newJob)
    {
        var currentJob = Job?.JobDetails.Responsibilities ?? 0;

        var toRemove = currentJob & ~newJob;
        foreach (var res in toRemove.GetFlags())
        {
            var typeDescription = TypeLibrary.GetType("Cinema.Jobs." + res.ToString());
            if (typeDescription is null) continue;

            Components.RemoveAny(typeDescription.TargetType);
        }

        // Filters to only have responsibilities that are in the new job but not the current job
        var toAdd = (newJob ^ currentJob) & newJob;

        foreach (var res in toAdd.GetFlags())
        {
            var typeDescription = TypeLibrary.GetType("Cinema.Jobs." + res.ToString());
            if (typeDescription is null) continue;

            var responsibility = typeDescription.Create<EntityComponent>();
            Components.Add(responsibility);
        }
    }

    //Job server ticking
    [GameEvent.Tick.Server]
    protected void JobServerTick()
    {
        //Handle any active fails
        if(Job.JobDetails.Fails > 0)
            HandleActiveJobFails();
    }

    /// <summary>
    /// Handles any active fails the employee currently has
    /// </summary>
    public void HandleActiveJobFails()
    {
        //Forgiveness timer hasn't finished
        if (Job.JobDetails.ForgiveTimer > 0.0f) return;

        //If there are recent fails and the timer has finished, forgive the employee for a fail
         SubtractJobFails();
    }

    /// <summary>
    /// Perform actions for a job task success
    /// </summary>
    public void SuccessJobTask()
    {
        //Give payment to the employee
        AddMoney(Job.JobDetails.PayRate);
    }

    /// <summary>
    /// Perform actions for a job task fail
    /// </summary>
    public void FailJobTask()
    {
        //The job won't take into account of fails, just stop here
        if (Job.JobDetails.FailAllowance == -1) return;

        //Reset forgiveness timer
        Job.JobDetails.ForgiveTimer = Job.JobDetails.ForgiveInterval;

        //Increment the fails
        Job.JobDetails.Fails++;

        //The employee has failed too many times past the allowance, fire them
        if (Job.JobDetails.Fails >= Job.JobDetails.FailAllowance)
            LeaveJob(true);
    }

    //Removes any active job fails
    private void SubtractJobFails()
    {
        //Decrement the total fails and reset the forgive timer to the forgive interval
        Job.JobDetails.Fails--;
        Job.JobDetails.ForgiveTimer = Job.JobDetails.ForgiveInterval;
    }

    /// <summary>
    /// Leaves the active job
    /// </summary>
    /// <param name="wasFired">Was the employee fired for bad performance?</param>
    public void LeaveJob(bool wasFired = false)
    {
        //The player's doesn't have a proper job (is a guest)
        if (Job.HasAbility(JobAbilities.Guest))
            throw new System.Exception("Tried to leave current job as a guest");

        if (wasFired)
            //180 seconds (3 minutes)
            TimeUntilStartNewJob = JobCooldownDuration * 6;
        else
            //If they left with any remaining fails, multiply with base cooldown duration    
            TimeUntilStartNewJob = JobCooldownDuration * Job.JobDetails.Fails;

        //^^^
        //Will have to add a property to the job details with a base cooldown
        //the cooldown here might break things -ItsRifter

        //TODO: Cleanup any task in progress

        //Remove active job and reset to default
        Components.RemoveAny<Jobs.PlayerJob>();
        Components.Add(Jobs.PlayerJob.GetDefaultJob());
    }

    //Developer command for setting jobs
    //IF NEEDED: add a parameter for setting job on a target player
    [ConCmd.Server("cinema.job.set")]
    public static void SetJob(int job = 0)
    {
        if (!CinemaGame.ValidateUser(ConsoleSystem.Caller.SteamId)) return;

        var player = ConsoleSystem.Caller.Pawn as Player;
        if ( player == null ) return;

        //Decrement the parameter so this can be used for array indexes
        job--;

        //the parameter is less than 0 or is past the total jobs
        //total job count is subtracted by 1 to prevent array out of bound indexes
        if (job < 0) return;
        if (job > Jobs.JobDetails.All.Count-1) return;

        //Set the job with the parameter index
        player.SetJob(Jobs.JobDetails.All[job]);

        //Log message to the host or dedicated console
        Log.Info($"Assigned to '{player.Job.Name}'");
    }

    //Leave the job, this is used either by console or from UI
    [ConCmd.Server("cinema.job.leave")]
    public static void JobLeaveCMD()
    {
        var player = ConsoleSystem.Caller.Pawn as Player;
        if (player == null) return;

        //The player's job is as a guest
        if (player.Job.Name == "Guest") return;

        //Leave the job
        player.LeaveJob();
    }
}
