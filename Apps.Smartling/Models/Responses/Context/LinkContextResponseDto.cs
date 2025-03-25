using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Smartling.Models.Responses.Context
{
    public class LinkContextResponseDto
    {
        public CreatedResponse Created { get; set; }
        public ErrorResponse Errors { get; set; }
    }

    public class CreatedResponse
    {
        public List<CreatedItem> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class CreatedItem
    {
        public string BindingUid { get; set; }
        public string ContextUid { get; set; }
        public string StringHashcode { get; set; }
        public List<string> Anchors { get; set; }
        public int ContextPosition { get; set; }
        public CoordinatesExtendedDto Coordinates { get; set; }
        public TimecodeDto Timecode { get; set; }
    }

    public class CoordinatesExtendedDto
    {
        public int Top { get; set; }
        public int Left { get; set; }
        public int Right { get; set; }
        public int Bottom { get; set; }
    }

    public class TimecodeDto
    {
        public int StartTime { get; set; }
        public int EndTime { get; set; }
    }

    public class ErrorResponse
    {
        public List<ErrorItem> Items { get; set; }
        public int TotalCount { get; set; }
    }

    public class ErrorItem
    {
        public string Message { get; set; }
    }
}
