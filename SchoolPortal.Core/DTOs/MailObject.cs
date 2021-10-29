using System.Collections.Generic;

namespace SchoolPortal.Core.DTOs
{
    public class MailObject
    {
        public IEnumerable<Recipient> Recipients { get; set; }
        public IEnumerable<string> AttachmentPaths { get; set; }
    }
}
