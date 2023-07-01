using System.IO.Enumeration;
using System;
using Cinema;
using Sandbox;
using Sandbox.UI;
using System.Collections.Generic;

public partial class PoopGameScreen : Panel
{
    public override bool HasContent => true;

    public Vector2 PoopPosition { get; set; } = new Vector2(0, 0);
    public Vector2 PoopVelocity { get; set; } = new Vector2(0, 0);
    public Vector2 PoopSize => new Vector2(80, 80);

    private Vector2 PlayArea => new Vector2(1600, 450);

    public PoopGameScreen()
    {
        Style.Height = Length.Percent(100);
        Style.Width = Length.Percent(100);
    }

    public override void Tick()
    {
        //PoopPosition = PoopPosition.WithX(0);

        if (Input.Down("jump"))
        {
            if (PoopVelocity.y < 0)
            {
                PoopVelocity = PoopVelocity.WithY(PoopVelocity.y / 2);
            }
            PoopVelocity += new Vector2(0, 1500) * Time.Delta;
        }
        else
        {
            if (PoopVelocity.y > 0)
            {
                PoopVelocity = PoopVelocity.WithY(PoopVelocity.y * 0.9f);
            }
            PoopVelocity += new Vector2(0, -1000) * Time.Delta;
        }

        PoopPosition += PoopVelocity * Time.Delta;

        PoopPosition = PoopPosition.Clamp(
            Vector2.Zero,
            new Vector2(PlayArea.x, PlayArea.y - PoopSize.y)
        );

        if (PoopPosition.y >= PlayArea.y - PoopSize.y)
        {
            PoopVelocity = PoopVelocity.WithY(0);
        }

        if (PoopPosition.y <= 0)
        {
            PoopVelocity = PoopVelocity.WithY(0);
        }

        PoopPosition += new Vector2(200, 0) * Time.Delta;
        if (PoopPosition.x + PoopSize.x >= PlayArea.x)
        {
            PoopPosition = new Vector2(0, PlayArea.y - PoopSize.y);
            PoopVelocity = Vector2.Zero;
        }
    }

    public override void DrawContent(ref RenderState state)
    {
        DrawRectangle(
            new Vector2(0, PlayArea.y - 300),
            new Vector2(300, 300),
            new Color(0.35f, 0.15f, 0.18f)
        );
        DrawRectangle(new Vector2(100, 20), new Vector2(600, 200), new Color(0.35f, 0.15f, 0.18f));
        DrawRectangle(new Vector2(500, 200), new Vector2(800, 200), new Color(0.35f, 0.15f, 0.18f));
        DrawRectangle(new Vector2(1100, 50), new Vector2(500, 200), new Color(0.35f, 0.15f, 0.18f));

        DrawRectangle(PoopPosition, PoopSize, Color.FromRgb(0xffffff), "textures/poopemoji.png");
    }

    private void DrawRectangle(Vector2 pos, Vector2 size, Color color, string texture = null)
    {
        var aspectRatio = PlayArea.x / PlayArea.y;
        var drawArea = new Vector2(Box.Rect.Size.y * aspectRatio, Box.Rect.Size.y);

        if (drawArea.x > Box.Rect.Size.x)
        {
            drawArea = new Vector2(Box.Rect.Size.x, Box.Rect.Size.x / aspectRatio);
        }

        //Log.Info($"drawArea: {drawArea} BoxSize {Box.Rect.Size}");
        var margin = Box.Rect.Size - drawArea;

        var relativePosition = pos / PlayArea;
        var relativeSize = size / PlayArea;

        var drawSpaceSize = drawArea;
        var actualPosition = drawSpaceSize * relativePosition;
        var actualSize = drawSpaceSize * relativeSize;

        var x = Box.Rect.TopLeft.x + actualPosition.x + margin.x / 2;
        var y = Box.Rect.BottomLeft.y - actualPosition.y - actualSize.y - margin.y / 2;
        Util.Draw.Reset();
        Util.Draw.Color = color;
        if (texture is not null)
        {
            Util.Draw.Texture = Texture.Load(FileSystem.Mounted, texture);
        }
        Util.Draw.Quad(new Rect(x, y, actualSize.x, actualSize.y));
    }
}
