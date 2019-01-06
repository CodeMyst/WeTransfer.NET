using Newtonsoft.Json;

namespace WeTransfer.NET
{
    public class FileInfo
    {
        [JsonProperty ("multipart")]
        public Multipart Multipart { get; set; }

        [JsonProperty ("size")]
        public int Size { get; set; }

        [JsonProperty ("type")]
        public string Type { get; set; }

        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string Path { get; set; }
        
        [JsonProperty ("id")]
        public string ID { get; set; }
    }
}