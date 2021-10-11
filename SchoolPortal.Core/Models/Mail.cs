namespace SchoolPortal.Core.Models
{
    public class Mail:BaseEntity
    {
        public string Email { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public bool IsSent { get; set; }
    }
}
