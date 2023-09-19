using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PleasantPOC.Models
{
    public class CredentialGroup
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid ParentId { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime Created { get; set; }
        public DateTime Modified { get; set;}
        public Dictionary<string, string> CustomUserFields { get; set; } = new();
        public Dictionary<string, string> CustomApplicationFields { get; set; } = new();
        public List<Attachment> Attachments { get; set; } = new();
        public List<CredentialGroup> Children { get; set; } = new();
        public List<Credential> Credentials { get; set; } = new();
        public List<Tag> Tags { get; set; } = new();
        public string UsageComment { get; set; } = string.Empty;
        public bool HasModifyEntriesAccess { get; set; }
        public bool HasViewEntryContentsAccess { get; set; }
        public CommentPrompts? CommentPrompts { get; set; }

    }
}
