using Sandbox;

namespace Cinema;

public partial class HotdogRoller
{
    public UI.Tooltip Tooltip { get; set; }
    public string Interaction { get; set; } = "Hotdog Roller";
    public string UseText { get; set; } = $"Press E to use";

    /// <summary>
    /// Sets up the UI when the machine is interacted with
    /// </summary>
    private void SetupUI()
    {
        Tooltip = new UI.Tooltip(this, UseText);
    }

    /// <summary>
    ///
    /// </summary>
    private void OnInteractionVolumeHover(string groupName)
    {
        switch(groupName)
        {
            case "hotdog_roller":
                Interaction = "Hotdog Roller";
                Tooltip.SetText($"{Interaction}");
                break;

            case "l_handle":
                Interaction = "Back Roller Temperature Knob";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;

            case "r_handle":
                Interaction = "Front Roller Temperature Knob";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;
            case "l_switch":
                Interaction = "Back Roller Power";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;
            case "r_switch":
                Interaction = "Front Roller Power";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;
            case "roller1":
            case "roller2":
            case "roller3":
            case "roller4":
            case "roller5":
            case "roller6":
            case "roller7":
            case "roller8":
            case "roller9":
            case "roller10":
                Interaction = "Roller";
                Tooltip.SetText($"{UseText} {Interaction}");
                break;

        }
    }

    /// <summary>
    ///
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
                    Tooltip.ShouldOpen(false);
                }
                else
                {
                    Tooltip.ShouldOpen(true);
                }
            }
            else
            {
                Tooltip.ShouldOpen(false);
            }
        }
    }

    /// <summary>
    ///
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
    ///
    /// </summary>
    private void DrawCursor(Vector3 pos)
    {
        DebugOverlay.Sphere(pos, 0.25f, Color.White);
    }
}
