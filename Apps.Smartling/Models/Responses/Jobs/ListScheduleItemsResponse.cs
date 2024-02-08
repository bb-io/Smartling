using Apps.Smartling.Models.Dtos.Jobs;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Responses.Jobs;

public record ListScheduleItemsResponse(
    [property: Display("Schedule items")] IEnumerable<ScheduleItemDto> ScheduleItems);