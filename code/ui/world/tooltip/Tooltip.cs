namespace Cinema.UI;

public partial class Tooltip
{
    private string Text = "";

    private bool Open { get; set; } = false;

    public Tooltip(string text)
    {
        Text = text;
    }

    public void ShouldOpen(bool open)
    {
        Open = open;

        StateHasChanged();
    }
}
