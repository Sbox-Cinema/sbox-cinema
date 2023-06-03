using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public string UseText { get; set; }

    /// <summary>
    /// Sets up the UI when the machine is interacted with
    /// </summary>
    private void SetupUI()
    {
        UseText = "Use Hotdog Roller";
    }

    /// <summary>
    /// Called when the player intersects with an interaction volume
    /// </summary>
    private void OnInteractionVolumeHover(string groupName)
    {
        switch(groupName)
        {
            case "hotdog_roller":
                UseText = "Hotdog Roller Info";
                break;
            case "l_handle":
                UseText = "Use Back Roller Temperature Knob";
                break;

            case "r_handle":
                UseText = "Use Front Roller Temperature Knob";
                break;
            case "l_switch":
                UseText = "Use Back Roller Power";
                break;
            case "r_switch":
                UseText = "Use Front Roller Power";
                break;
            case "roller1":
                UseText = "Add Front Roller Hotdog";
                break;
            case "roller6":
                UseText = "Add Back Roller Hotdog";
                break;
        }
    }

    /// <summary>
    /// Raycast to see if player is in contact with any interactables
    /// </summary>
    [GameEvent.Tick]
    public void Update()
    {
        if (Game.LocalPawn is Player player)
        {
            TraceResult tr = Trace.Ray(player.AimRay, 2000)
                            .EntitiesOnly()
                            .Ignore(player)
                            .WithoutTags("cookable")
                            .Run();

            if (tr.Hit)
            {
                foreach (var body in PhysicsGroup.Bodies)
                {
                    if (body.GroupName != "" && body.GroupName != tr.Body.GroupName)
                    {
                        DrawVolume(body, false);
                    }
                    else if (body.GroupName != "")
                    {
                        DrawVolume(body, true);
                        DrawCursor(tr.EndPosition);

                        OnInteractionVolumeHover(body.GroupName);
                    }
                }

                if (tr.Body.GroupName == "")
                {
                    UseText = "Use Hotdog Roller";
                }
            }
          
        }
    }

    /// <summary>
    /// Draws interaction volume
    /// </summary>
    private void DrawVolume(PhysicsBody body, bool active)
    {
        BBox bbox = body.GetBounds();

        foreach (var pt in bbox.Corners)
        {
            var pt2 = pt.Cross(bbox.Center).Normal;
            
            var planePos = bbox.Center;
            var planeNormal = planePos + new Vector3(0, 0, 1);

            float distance = planeNormal.Dot(pt - planePos);

            float lineLength = 0.5f;

            Color inactiveColor = Color.Gray;
            Color activeColor = Color.White;

            //Back Points
            if(distance < 0)
            {
                //Bottom 
                if (pt2.x < 0)
                {
                    // Right
                    if (pt2.y < 0)
                    {
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Red);

                        DebugOverlay.Line(pt, pt + (Vector3.Backward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Right * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Up * lineLength), !active ? inactiveColor : activeColor);
                    }
                    // Left
                    if (pt2.y > 0)
                    { 
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Green);

                        DebugOverlay.Line(pt, pt + (Vector3.Forward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Right * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Up * lineLength), !active ? inactiveColor : activeColor);
                    }
                }

                // Top
                if (pt2.x > 0)
                {
                    // Right
                    if (pt2.y < 0)
                    {
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Cyan);

                        DebugOverlay.Line(pt, pt + (Vector3.Backward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Right * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Down * lineLength), !active ? inactiveColor : activeColor);
                    }

                    // Left
                    if (pt2.y > 0)
                    {
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Magenta);

                        DebugOverlay.Line(pt, pt + (Vector3.Forward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Right * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Down * lineLength), !active ? inactiveColor : activeColor);
                    }

                }
            }
            
            // Front points
            if (distance > 0)
            {
                // Bottom 
                if (pt2.x < 0)
                {
                    // Right
                    if (pt2.y < 0)
                    {
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Red);

                        DebugOverlay.Line(pt, pt + (Vector3.Backward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Left * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Up * lineLength), !active ? inactiveColor : activeColor);
                    }

                    // Left
                    if (pt2.y > 0)
                    {
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Green);

                        DebugOverlay.Line(pt, pt + (Vector3.Forward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Left * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Up * lineLength), !active ? inactiveColor : activeColor);
                    }
                }

                // Top
                if (pt2.x > 0)
                {
                    // Right
                    if (pt2.y < 0)
                    {
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Cyan);

                        DebugOverlay.Line(pt, pt + (Vector3.Backward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Left * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Down * lineLength), !active ? inactiveColor : activeColor);
                    }

                    // Left
                    if (pt2.y > 0)
                    {
                        //DebugOverlay.Sphere(pt, 0.15f, Color.Magenta);

                        DebugOverlay.Line(pt, pt + (Vector3.Forward * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Left * lineLength), !active ? inactiveColor : activeColor);
                        DebugOverlay.Line(pt, pt + (Vector3.Down * lineLength), !active ? inactiveColor : activeColor);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Draws interaction cursor
    /// </summary>
    private void DrawCursor(Vector3 pos)
    {
        DebugOverlay.Sphere(pos, 0.25f, Color.White);
    }
}
