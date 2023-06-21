using Sandbox;

namespace Cinema;
public partial class CinemaGame
{
    public override void OnVoicePlayed(IClient cl)
    {
        cl.Voice.WantsStereo = true;

        base.OnVoicePlayed(cl);
    }
}
