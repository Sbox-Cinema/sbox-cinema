using Sandbox;
using System;

namespace Cinema;

public partial class AstroTurferBotController : EntityComponent<Player>, ISingletonComponent
{
    private CinemaZone LastEnteredZone { get; set; }
    [ConCmd.Server("bot.astro.debug")]
    public static void BeginDebug()
    {
        if (ConsoleSystem.Caller?.Pawn is not Player ply)
            return;

        Log.Info("ASTROTURFER BOT CONTROLS ENABLED");
        Log.Info("\tMOUSE3: Spawn bot in current theater");
        Log.Info("\tSHIFT + MOUSE3: Spawn bot in last entered theater");
        Log.Info("\tKP_ENTER: Queue media");
        Log.Info("\tKP_PLUS: Like media"); 
        Log.Info("\tKP_MINUS: Dislike media");
        ply.Components.Create<AstroTurferBotController>();
    }

    protected override void OnActivate() => Entity.ZoneEntered += EnteredZone;
    protected override void OnDeactivate() => Entity.ZoneEntered -= EnteredZone;
    protected void EnteredZone(object sender, CinemaZone zone) => LastEnteredZone = zone;

    [GameEvent.Client.BuildInput]
    public void OnBuildInput()
    {
        if (Input.Pressed("BotSpawn"))
        {
            AstroTurferBot.SpawnAstroTurfer();
        }
        if (Input.Pressed("BotQueue"))
        {
            var currentZone = Entity.GetCurrentTheaterZone();
            if (currentZone == null && Input.Down("Run"))
                currentZone = LastEnteredZone;
            AstroTurferBot.QueueTestMedia(currentZone?.NetworkIdent ?? 0);
        }
        if (Input.Pressed("BotLike"))
        {
            AstroTurferBot.LikeCurrentMedia();
        }
        if (Input.Pressed("BotDislike"))
        {
            AstroTurferBot.DislikeCurrentMedia();
        }
    }
}
