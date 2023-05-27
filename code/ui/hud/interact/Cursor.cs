namespace Cinema.UI;
public partial class Cursor
{
    private bool Visible { get; set; } = true;
 
    public void ShouldOpen(bool open)
    {
        Visible = open;

        StateHasChanged();
    }
}
