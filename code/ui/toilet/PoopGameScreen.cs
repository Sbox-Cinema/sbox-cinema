using System;
using Cinema;
using Cinema.UI.SpriteGame;
using Sandbox;
using Sandbox.UI;

public partial class PoopGameScreen : Panel
{
    public override bool HasContent => true;

    public Action OnWin { get; set; }

    public Action OnLose { get; set; }

    private Engine Engine { get; set; }

    public PoopGameScreen()
    {
        Style.Height = Length.Percent(100);
        Style.Width = Length.Percent(100);
    }

    public void StartGame()
    {
        Engine = new Engine() { ViewPort = Box.Rect };
        Engine
            .CreateEntity<SpriteEntity>()
            .SetSize(Engine.PlayArea)
            .SetColor(Color.FromRgb(0x521b1b))
            .SetZIndex(-100);
        var poop = Engine.CreateEntity<Poop>();
        poop.SetTexture(Texture.Load(FileSystem.Mounted, "textures/poopemoji.png"))
            .SetSize(new Vector2(40, 40))
            .SetPosition(new Vector2(0, Engine.PlayArea.y - 120));
        poop.OnWin = OnWin;
        poop.OnLose = OnLose;
        poop.Velocity = new Vector2(200, 0);

        var numKinks = 7;
        var totalLength = Engine.PlayArea.x;
        var averageKinkSize = totalLength / numKinks;
        var kinkX = -averageKinkSize / 2;

        for (var i = 0; i < numKinks; ++i)
        {
            var isTop = i % 4 == 0;
            var isMid = i % 4 == 1 || i % 4 == 3;

            var kinkLength = averageKinkSize * Game.Random.Float(1.5f, 1.8f);
            var kinkHeight = 150;

            var yPosition = 20;
            if (isMid)
                yPosition += 100;
            if (isTop)
                yPosition += 250;

            if (i == 0)
            {
                kinkLength = 400;
                kinkHeight += 250;
                yPosition -= 250;
            }

            Engine
                .CreateEntity<Intestine>()
                .SetPosition(kinkX, yPosition)
                .SetSize(kinkLength, kinkHeight)
                .SetColor(new Color(0.35f, 0.15f, 0.18f))
                .SetZIndex(-1);

            kinkX += averageKinkSize;
        }
    }

    public override void OnHotloaded()
    {
        StartGame();
    }

    public override void Tick()
    {
        Engine?.Update();
    }

    public override void DrawContent(ref RenderState state)
    {
        if (Engine is null)
            return;

        Engine.ViewPort = Box.Rect;
        Engine.Draw();
    }
}
