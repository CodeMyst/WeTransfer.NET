using System;

namespace WeTransfer.NET
{
    public class FileUploadResult
    {
        public string URL { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}