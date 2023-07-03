namespace Cinema;

public partial class CinemaZone
{
    /// <summary>
    /// Audio channel conforming to MP3/WAV/FLAC part of ANSI/CEA-863-A.
    /// </summary>
    public enum AudioChannel : int
    {
        FrontLeft = 0,
        FrontRight,
        Center,
        Subwoofer,
        SideLeft,
        SideRight,
        RearLeft,
        RearRight
    }
}
