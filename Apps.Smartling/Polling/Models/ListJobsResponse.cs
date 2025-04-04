namespace Apps.Smartling.Polling.Models
{
    public class ListJobsResponse
    {
        public ResponseData Response { get; set; }
    }
    public class ResponseData
    {
        public string Code { get; set; }
        public DataContent Data { get; set; }
    }

    public class DataContent
    {
        public List<JobItem> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class JobItem
    {
        public DateTime CreatedDate { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        public string JobName { get; set; }
        public string JobNumber { get; set; }
        public string JobStatus { get; set; }
        public string ReferenceNumber { get; set; }
        public List<string> TargetLocaleIds { get; set; }
        public string TranslationJobUid { get; set; }
    }
}
