using Newtonsoft.Json;

namespace WeTransfer.NET
{
    public class FileTransfer
    {
        [JsonProperty ("message")]
        public string Message { get; set; }
        
        [JsonProperty ("files")]
        public PartialFileInfo [] Files { get; set; }
    }
}