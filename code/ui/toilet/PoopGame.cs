using System;
using Sandbox;
using Sandbox.UI;

namespace Cinema.UI;

public partial class PoopGame : Panel, IMenuScreen
{
    public enum State
    {
        Instructions,
        Playing,
        Pooping
    }

    public State GameState { get; set; }

    public static PoopGame Instance { get; set; }
    public bool IsOpen { get; private set; }

    protected string VisibleClass => IsOpen ? "visible" : "";
    protected string GameVisibleClass => GameState == State.Playing ? "visible" : "";

    protected static string JumpButtonName => Input.GetButtonOrigin("jump");

    public string Name => "Poop Game";

    public Panel GameWindow { get; set; }

    public PoopGameScreen Game { get; set; }

    public int TriesRemaining { get; protected set; } = 3;

    protected int[] TryArray => new int[Math.Max(TriesRemaining, 0)];

    public Action<Toilet.UsageLevel> UseToiletCallback { get; set; }

    public Sound GameMusic { get; set; }

    protected static string SongName = "bathroom-song";

    protected static string[] PoopText =>
        new string[] { "Disappointing", "Okay", "Good", "Great", "Excellent" };

    public PoopGame()
    {
        Instance = this;
    }

    public void OnWinGame()
    {
        Game.Delete();
        ShowGameOver();
    }

    public void OnLoseGame()
    {
        TriesRemaining -= 1;

        if (TriesRemaining >= 0)
        {
            GameMusic.Stop();
            GameMusic = Sandbox.Game.LocalPawn.PlaySound(SongName);
            Game.StartGame();
            return;
        }

        Game.Delete();
        ShowGameOver();
    }

    protected void ShowGameOver()
    {
        var stopMusic = async () =>
        {
            await GameTask.DelayRealtimeSeconds(0.15f);
            GameMusic.Stop();
        };
        stopMusic();

        GameState = State.Pooping;
        var level = Toilet.UsageLevel.Normal;
        if (TriesRemaining > 1)
            level = Toilet.UsageLevel.Big;
        if (TriesRemaining == 3)
            level = Toilet.UsageLevel.Excellent;

        UseToiletCallback?.Invoke(level);
    }

    public bool Open()
    {
        IsOpen = true;

        GameState = State.Instructions;

        return true;
    }

    public void Close()
    {
        IsOpen = false;
    }

    public override void Tick()
    {
        if (GameState == State.Instructions && Input.Pressed("jump"))
        {
            StartGame();
        }
    }

    protected void StartGame()
    {
        TriesRemaining = 3;
        GameState = State.Playing;
        Game = GameWindow.AddChild<PoopGameScreen>();
        Game.OnWin = OnWinGame;
        Game.OnLose = OnLoseGame;
        Game.StartGame();

        GameMusic.Stop();
        GameMusic = Sandbox.Game.LocalPawn.PlaySound(SongName);
    }

    protected override int BuildHash()
    {
        return HashCode.Combine(IsOpen, TriesRemaining, GameState);
    }
}
