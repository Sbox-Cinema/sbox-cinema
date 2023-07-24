using Sandbox;

namespace Cinema;

public partial class AstroTurferBotController : EntityComponent<Player>, ISingletonComponent
{
    [ConCmd.Server("bot.astro.debug")]
    public static void BeginDebug()
    {
        if (ConsoleSystem.Caller?.Pawn is not Player ply)
            return;

        ply.Components.Create<AstroTurferBotController>();
    }
    
    [GameEvent.Client.BuildInput]
    public void OnBuildInput()
    {
        if (Input.Pressed("BotSpawn"))
        {
            AstroTurferBot.SpawnAstroTurfer();
        }
        if (Input.Pressed("BotQueue"))
        {
            AstroTurferBot.QueueTestMedia();
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
