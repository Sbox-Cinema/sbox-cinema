using Sandbox;
using Sandbox.UI;
using System;

namespace Cinema.UI;

public partial class InteractPrompt : Panel
{
    public static InteractPrompt Current;

    public static Player Player => Game.LocalPawn as Player;

    protected Panel UseBar { get; set; }

    public float LongUsePercentage { get; set; }

    protected string PromptText { get; set; } = "";

    protected bool Hidden { get; set; } = true;

    public InteractPrompt()
    {
        Current = this;
    }

    public override void Tick()
    {
        UseBar.Style.Width = Length.Percent(LongUsePercentage * 100);
        if (Player is null) return;

        var ent = Player.FindUsable(true);
        if (!ent.IsValid())
        {
            HidePrompt();
            return;
        }

        var hold = false;
        var verb = "Use";
        var percent = 0f;

        if (ent is ICinemaUse cinemaUse)
        {
            if (!cinemaUse.IsUsable(Player))
            {
                SetCannotUsePrompt(cinemaUse.CannotUseText);
                return;
            }
            hold = cinemaUse.TimedUse;
            verb = cinemaUse.UseText;
            percent = hold ? cinemaUse.TimedUsePercentage : 0;
        }

        SetPrompt(hold, verb, percent);
    }

    public void SetPrompt(bool hold, string text, float percent)
    {
        var action = hold ? "Hold" : "Press";
        var useKey = Input.GetButtonOrigin("use");
        PromptText = $"{action} {useKey.ToUpper()} to {text}";
        Hidden = false;
        LongUsePercentage = percent;
    }

    public void SetCannotUsePrompt(string text)
    {
        PromptText = text;
        Hidden = false;
        LongUsePercentage = 0;
    }

    public void HidePrompt()
    {
        Hidden = true;
    }

    protected override int BuildHash()
    {
        return HashCode.Combine(PromptText, Hidden, LongUsePercentage);
    }
}
