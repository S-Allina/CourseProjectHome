namespace Main.Presentation.MVC.ViewModel
{
    public record DeleteSelectedViewModel
    {
        public required int[] SelectedIds { get; init; }
    }
}
