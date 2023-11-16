using Apps.Smartling.Models.Dtos.Attachments;

namespace Apps.Smartling.Models.Responses.Attachments;

public record ListAttachmentsResponse(IEnumerable<AttachmentDto> Attachments);