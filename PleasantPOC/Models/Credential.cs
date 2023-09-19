namespace PleasantPOC.Models
{
    public class Credential
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
        public string Notes { get; set; } = string.Empty;
        public Guid GroupId { get; set; }
        public DateTime Created { get; set; }
        public DateTime Modified { get; set; }
        public DateTime? Expires { get; set; }
        public Dictionary<string, string> CustomUserFields { get; set; } = new();
        public Dictionary<string, string> CustomApplicationFields { get; set; } = new();
        public List<Attachment> Attachments { get; set; } = new();
        public string UssageComment { get; set; } = string.Empty;
        public List<Tag> Tags { get; set; } = new();
        public bool HasModifyEntriesAccess { get; set; }
        public bool HasViewEntryContentsAccess { get; set; }
        public CommentPrompts? CommentPrompts { get; set; }
    }
}