﻿using Blackbird.Applications.Sdk.Common;

namespace Apps.Smartling.Models.Dtos.Attachments;

public class AttachmentDto
{
    [Display("Attachment ID")]
    public string AttachmentUid { get; set; }
    
    public string Name { get; set; }
    
    public string? Description { get; set; }
    
    [Display("Date of creation")]
    public DateTime CreatedDate { get; set; }
    
    [Display("Created by user ID")]
    public string CreatedByUserUid { get; set; }
}