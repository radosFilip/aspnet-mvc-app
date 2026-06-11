namespace Filip_Rados_lab5.ViewModels
{
    public class DateTimeInputModel
    {
        public string Name { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
        public DateTime? Value { get; set; }
        public bool Required { get; set; } = true;
    }
}
