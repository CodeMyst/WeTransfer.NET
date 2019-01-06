using Newtonsoft.Json;

namespace WeTransfer.NET
{
    public class FileTransferResponse
    {
        [JsonProperty ("success")]
        public bool Success { get; set; }

        [JsonProperty ("id")]
        public string ID { get; set; }

        [JsonProperty ("message")]
        public string Message { get; set; }
        
        [JsonProperty ("state")]
        public string State { get; set; }

        [JsonProperty ("url")]
        public string Url { get; set; }

        [JsonProperty ("expires_at")]
        public string ExpiresAt { get; set; }
        
        [JsonProperty ("files")]
        public FileInfo [] Files { get; set; }
    }
}