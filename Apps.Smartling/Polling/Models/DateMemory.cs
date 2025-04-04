namespace Apps.Smartling.Polling.Models
{
    public class DateMemory
    {
        public DateTime LastInteractionDate { get; set; }

        public List<string>? KnownJobIds { get; set; } = new List<string>();
    }
}
