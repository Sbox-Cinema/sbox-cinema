using Sandbox;

namespace Cinema;

public partial class Player
{
    public void SimulateCamera(IClient cl)
    {
        Camera.Position = EyePosition;
        Camera.Rotation = EyeRotation;
        Camera.ZNear = 0.5f;

        // Set field of view to whatever the user chose in options
        Camera.FieldOfView = Screen.CreateVerticalFieldOfView(Sandbox.Game.Preferences.FieldOfView);

        // Set the first person viewer to this, so it won't render our model
        Camera.FirstPersonViewer = this;

        Camera.Main.SetViewModelCamera(Camera.FieldOfView);

        if ( ActiveChild is Carriable carriable )
        {
            carriable.UpdateViewmodelCamera();
            carriable.UpdateCamera();
        }
    }
}
