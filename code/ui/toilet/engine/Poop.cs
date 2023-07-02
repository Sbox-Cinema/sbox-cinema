using System;
using System.Linq;
using Sandbox;

namespace Cinema.UI.SpriteGame;

public partial class Poop : SpriteEntity
{
    public Action OnLose { get; set; }
    public Action OnWin { get; set; }

    public Poop() : base()
    {
        Texture = Texture.Load(FileSystem.Mounted, "textures/poopemoji.png");
    }

    private void DebugMovement()
    {
        var speed = 200;
        if (Input.Down("Right"))
        {
            Position += new Vector2(1, 0) * speed * Time.Delta;
        }

        if (Input.Down("Forward"))
        {
            Position += new Vector2(0, 1) * speed * Time.Delta;
        }

        if (Input.Down("Backward"))
        {
            Position += new Vector2(0, -1) * speed * Time.Delta;
        }

        if (Input.Down("Left"))
        {
            Position += new Vector2(-1, 0) * speed * Time.Delta;
        }
    }

    public override void Update()
    {
        if (Input.Down("jump"))
        {
            if (Velocity.y < 0)
            {
                Velocity = Velocity.WithY(Velocity.y / 2);
            }
            Velocity += new Vector2(0, 1500) * Time.Delta;
        }
        else
        {
            if (Velocity.y > 0)
            {
                Velocity = Velocity.WithY(Velocity.y * 0.9f);
            }
            Velocity += new Vector2(0, -800) * Time.Delta;
        }

        var boundingSize = Size * 0.5f;
        var marginSize = (Size - boundingSize) / 2;

        var intestines = Engine.Entities.OfType<Intestine>();
        var insideX = intestines.Where(e =>
        {
            return e.Position.x <= Position.x + marginSize.x
                && e.Position.x + e.Size.x >= Position.x + marginSize.x + boundingSize.x;
        });
        var insideY = insideX.Where(e =>
        {
            return e.Position.y <= Position.y + marginSize.y
                && e.Position.y + e.Size.y >= Position.y + marginSize.y + boundingSize.y;
        });
        var partialInsideY = insideX.Where(e =>
        {
            return e.Position.y <= Position.y + Size.y && e.Position.y + e.Size.y >= Position.y;
        });

        var isInside = insideY.Any();
        var isPartiallyInsideMultiple = partialInsideY.Count() > 1;

        if (!isInside && !isPartiallyInsideMultiple)
        {
            OnLose?.Invoke();
            return;
        }

        if (Position.x > Engine.PlayArea.x - Size.x * 4)
        {
            OnWin?.Invoke();
        }
    }
}
