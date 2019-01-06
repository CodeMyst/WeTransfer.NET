using Newtonsoft.Json;

namespace WeTransfer.NET
{
    public class PartialFileInfo
    {
        [JsonProperty ("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string Path { get; set; }
        
        [JsonProperty ("size")]
        public int Size { get; set; }
    }
}