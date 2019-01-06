using Newtonsoft.Json;

namespace WeTransfer.NET
{
    public class Multipart
    {
        [JsonProperty ("part_numbers")]
        public int PartNumbers { get; set; }
     
        [JsonProperty ("chunk_size")]
        public int ChunkSize { get; set; }
    }
}