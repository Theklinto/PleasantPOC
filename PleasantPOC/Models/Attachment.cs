using System.Text.Json.Serialization;

namespace PleasantPOC.Models
{
    public class Attachment
    {
        [JsonPropertyName("CredentialObjectId")]
        public Guid CredentialId { get; set; }
        public string Name { get; set; } = string.Empty;
        public byte[] FileData { get; set; } = Array.Empty<byte>();
    }
}