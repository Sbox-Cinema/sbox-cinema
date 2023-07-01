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
        PoopPosition = PoopPosition.WithX(0);

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
    }

    public Vector2[] BottomTunnelVertices { get; set; } = Array.Empty<Vector2>();

    [Event.Hotload]
    public override void OnHotloaded()
    {
        Log.Info("Hotloading PoopGameScreen");

        var waves = 2;
        var granularity = 0.1f;
        var scale = PlayArea.x / waves;
        var height = 40f;

        var vertices = new List<Vector2>();

        var lastX = 0f;
        var lastY = height;

        vertices.Add(new Vector2(lastX, 0));
        vertices.Add(new Vector2(lastX, lastY));
        vertices.Add(new Vector2(lastX + granularity * scale, 0));

        for (var x = granularity; x < waves; x += granularity)
        {
            var y = (float)Math.Sin(x * Math.PI * 2) * height + height;

            var realX = x * scale;
            vertices.Add(new Vector2(lastX, lastY));
            vertices.Add(new Vector2(realX, y));
            vertices.Add(new Vector2(realX, 0));

            vertices.Add(new Vector2(lastX, 0));
            vertices.Add(new Vector2(lastX, lastY));
            vertices.Add(new Vector2(lastX + granularity * scale, 0));

            lastX = realX;
            lastY = y;
        }

        BottomTunnelVertices = vertices.ToArray();
    }

    public override void DrawContent(ref RenderState state)
    {
        //DrawRectangle(Vector2.Zero, PlayArea, new Color(0.35f, 0.15f, 0.18f));
        DrawRectangle(PoopPosition, PoopSize, Color.FromRgb(0xffffff));
        DrawVertices(BottomTunnelVertices, Color.FromRgb(0xffffff));
    }

    private void DrawRectangle(Vector2 pos, Vector2 size, Color color)
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
        //Log.Info($"actualPosition: {actualPosition} actualSize: {actualSize}");



        var x = Box.Rect.TopLeft.x + actualPosition.x + margin.x / 2;
        var y = Box.Rect.BottomLeft.y - actualPosition.y - actualSize.y - margin.y / 2;
        Util.Draw.Reset();
        Util.Draw.Color = color;
        var poop = Texture.Load(FileSystem.Mounted, "textures/poopemoji.png");
        Util.Draw.Texture = poop;
        Util.Draw.Quad(new Rect(x, y, actualSize.x, actualSize.y));
    }

    private void DrawVertices(Vector2[] vertices, Color color)
    {
        var aspectRatio = PlayArea.x / PlayArea.y;
        var drawArea = new Vector2(Box.Rect.Size.y * aspectRatio, Box.Rect.Size.y);

        if (drawArea.x > Box.Rect.Size.x)
        {
            drawArea = new Vector2(Box.Rect.Size.x, Box.Rect.Size.x / aspectRatio);
        }

        var margin = Box.Rect.Size - drawArea;
        var drawSpaceSize = drawArea;

        Util.Draw.Reset();
        Util.Draw.Color = color;

        Util.Draw.MeshStart();
        foreach (var vert in vertices)
        {
            var relativePosition = vert / PlayArea;
            var actualPosition = drawSpaceSize * relativePosition;
            var x = Box.Rect.TopLeft.x + actualPosition.x + margin.x / 2;
            var y = Box.Rect.BottomLeft.y - actualPosition.y - margin.y / 2;
            Util.Draw.AddVertex(new Vector2(x, y), color);
        }
        Util.Draw.MeshEnd();
    }
}
