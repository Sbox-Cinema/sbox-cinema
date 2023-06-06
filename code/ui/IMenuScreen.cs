namespace Cinema.UI;

public interface IMenuScreen
{
    public bool IsOpen { get; }
    public string Name { get; }

    public bool Open();
    public void Close();
}
