using System.Linq;
using System.Collections.Generic;
using Sandbox;

namespace Cinema.UI.SpriteGame;

public partial class Engine
{
    public Rect ViewPort { get; set; }

    public Vector2 PlayArea => new Vector2(1800, 440);

    public float AspectRatio => PlayArea.y / PlayArea.x;

    public Vector2 DrawArea => CalculateDrawArea();

    public List<SpriteEntity> Entities { get; set; } = new();

    public T CreateEntity<T>() where T : SpriteEntity, new()
    {
        var entity = new T { Engine = this };
        Entities.Add(entity);
        return entity;
    }

    public void Update()
    {
        foreach (var entity in Entities)
        {
            entity.Update();

            if (entity.Velocity.LengthSquared <= 1)
                continue;
            var newPosition = entity.Position + entity.Velocity * Time.Delta;
            entity.SetPosition(newPosition);
        }
    }

    public void Draw()
    {
        Entities = Entities.OrderBy(x => x.ZIndex).ToList();
        foreach (var entity in Entities)
        {
            entity.Draw();
        }
    }

    public Vector2 CalculateDrawArea()
    {
        var drawArea = new Vector2(ViewPort.Size.x, AspectRatio * ViewPort.Size.x);
        //Log.Info($"drawArea: {drawArea} vs {ViewPort.Size} ({AspectRatio})");

        if (drawArea.y > ViewPort.Size.y)
        {
            drawArea = new Vector2(ViewPort.Size.y / AspectRatio, ViewPort.Size.y);
        }

        return drawArea;
    }

    public Vector2 DrawOffset()
    {
        var drawArea = CalculateDrawArea();

        var margin = ViewPort.Size - drawArea;

        var x = ViewPort.TopLeft.x + margin.x / 2;
        var y = ViewPort.BottomLeft.y - margin.y / 2;

        return new Vector2(x, y);
    }

    public void DrawRectangle(Vector2 pos, Vector2 size, Color color, Texture texture = null)
    {
        var offset = DrawOffset();

        var scaledPosition = DrawArea * (pos / PlayArea);
        var scaledSize = DrawArea * (size / PlayArea);

        var drawPosition = new Vector2(
            offset.x + scaledPosition.x,
            offset.y - scaledPosition.y - scaledSize.y
        );

        Util.Draw.Reset();
        Util.Draw.Color = color;
        Util.Draw.Texture = texture ?? Texture.White;
        var drawRect = new Rect(drawPosition.x, drawPosition.y, scaledSize.x, scaledSize.y);
        //Log.Info(drawRect);
        Util.Draw.Quad(drawRect);
    }
}
