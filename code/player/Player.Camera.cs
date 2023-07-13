using Sandbox;

namespace Cinema;

public partial class Player
{
    // default
    public bool isRightShoulderView;

    public void ThirdPersonSwapShoulder()
    {
    if (!ThirdPersonCamera)
        return;

        isRightShoulderView = !isRightShoulderView;
    }

    public void SimulateCamera(IClient cl)
    {
        Camera.Rotation = EyeRotation;

        // Set field of view to whatever the user chose in options
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Game.Preferences.FieldOfView);

        if (ThirdPersonCamera)
        {
            Camera.FirstPersonViewer = null;

            Vector3 targetPos;
            var center = Position + Vector3.Up * 80;
            var pos = center;
            var rot = Rotation.FromAxis(Vector3.Up, 0) * Camera.Rotation;

            float distance = 130.0f * Scale;
            targetPos = pos + rot.Right * ((CollisionBounds.Mins.x + 32) * Scale);
            targetPos = pos + rot.Right * ((isRightShoulderView ? 1 : -1) * 32 * Scale);

            targetPos += rot.Forward * -distance;

            var tr = Trace.Ray(pos, targetPos)
                .WithAnyTags("solid")
                .Ignore(this)
                .Radius(8)
                .Run();

            Camera.Position = tr.EndPosition;
        }
        else
        {
            Camera.Position = EyePosition;
            Camera.ZNear = 0.5f;

            // Set the first person viewer to this, so it won't render our model
            Camera.FirstPersonViewer = this;
            Camera.Main.SetViewModelCamera(Camera.FieldOfView);

            if (ActiveChild is Carriable carriable)
            {
                carriable.UpdateViewmodelCamera();
                carriable.UpdateCamera();
            }
        }


    }
}
