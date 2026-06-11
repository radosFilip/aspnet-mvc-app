namespace Filip_Rados_lab5.ViewModels
{
    public class AutocompleteInputModel
    {
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public string SearchUrl { get; set; } = string.Empty;
        public int? SelectedId { get; set; }
        public string? SelectedText { get; set; }
        public string Placeholder { get; set; } = "Pocnite pisati...";
        public bool Required { get; set; } = true;
    }
}
