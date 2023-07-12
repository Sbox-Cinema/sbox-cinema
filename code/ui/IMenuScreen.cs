namespace Cinema.UI;

public interface IMenuScreen
{
    public bool IsOpen { get; }
    public string Name { get; }
    public bool ShouldHideHud => false;

    public bool Open();
    public void Close();
}
