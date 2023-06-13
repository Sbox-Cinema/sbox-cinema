using System;

namespace Cinema;

public class PlayingMedia : IEquatable<PlayingMedia>
{
    public virtual string Url { get; set; }

    public virtual bool IsEqual(PlayingMedia other)
    {
        return Url == other.Url;
    }

    public bool Equals(PlayingMedia other)
    {
        return IsEqual(other);
    }

    public override bool Equals(object obj)
    {
        return Equals(obj as PlayingMedia);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Url);
    }

    public static bool operator ==(PlayingMedia m1, PlayingMedia m2)
    {
        if (m1 is null)
            return m2 is null;

        return m1.Equals(m2);
    }

    public static bool operator !=(PlayingMedia m1, PlayingMedia m2)
    {
        return !(m1 == m2);
    }
}
