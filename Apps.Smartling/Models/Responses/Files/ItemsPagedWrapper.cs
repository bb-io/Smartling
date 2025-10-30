namespace Apps.Smartling.Models.Responses.Files
{
    public class ItemsPagedWrapper<T>
    {
        public IEnumerable<T> Items { get; set; } = Array.Empty<T>();
        public int TotalCount { get; set; }
    }
}
