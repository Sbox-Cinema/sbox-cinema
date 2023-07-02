using Sandbox;

namespace Cinema.UI.SpriteGame;

public partial class SpriteEntity
{
    public Engine Engine { get; set; }
    public Vector2 Position { get; set; } = new(0, 0);
    public Vector2 Size { get; set; } = new(32, 32);
    public float ZIndex { get; set; } = 0;
    public Vector2 Velocity { get; set; } = new(0, 0);
    public Texture Texture { get; set; } = Texture.White;
    public Color Color { get; set; } = Color.White;

    public SpriteEntity() { }

    public SpriteEntity(Engine engine)
    {
        Engine = engine;
    }

    public SpriteEntity SetPosition(Vector2 pos)
    {
        Position = pos;
        return this;
    }

    public SpriteEntity SetPosition(float x, float y)
    {
        Position = new Vector2(x, y);
        return this;
    }

    public SpriteEntity SetSize(Vector2 size)
    {
        Size = size;
        return this;
    }

    public SpriteEntity SetSize(float x, float y)
    {
        Size = new Vector2(x, y);
        return this;
    }

    public SpriteEntity SetZIndex(float zIndex)
    {
        ZIndex = zIndex;
        return this;
    }

    public SpriteEntity SetVelocity(float vel)
    {
        Velocity = vel;
        return this;
    }

    public SpriteEntity SetTexture(Texture tex)
    {
        Texture = tex;
        return this;
    }

    public SpriteEntity SetColor(Color col)
    {
        Color = col;
        return this;
    }

    public virtual void Draw()
    {
        Engine.DrawRectangle(Position, Size, Color, Texture);
    }

    public virtual void Update() { }
}
