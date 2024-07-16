namespace Apps.Smartling.Models.Dtos
{
    public class AsyncProcessResult
    {
        public string ProcessUid { get; set; }
        public string ProcessState { get; set; }
        public string ProcessType { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}
