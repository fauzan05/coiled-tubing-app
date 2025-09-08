namespace coiled_tubing_app.Models
{
    public class ChartItem
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public bool IsSelected { get; set; } = false;
    }
}