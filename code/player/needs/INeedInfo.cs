namespace Cinema;

public interface INeedInfo
{
    public string DisplayName { get; }
    public string IconPath { get; }
    public float SatisfactionPercent { get; }
}
