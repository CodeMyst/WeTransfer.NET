using System;
using System.IO;
using System.Threading.Tasks;

using NUnit.Framework;

namespace WeTransfer.NET.Tests
{
    public class Tests
    {
        [Test]
        public async Task UploadFile ()
        {
            string key = Environment.GetEnvironmentVariable ("WE_TRANSFER_KEY");

            Assert.IsFalse (string.IsNullOrEmpty (key), "You have to provide your WeTransfer API key as an environment variable WE_TRANSFER_KEY.");

            string fileName = "TestFile.png";
            string path = Path.Combine (Directory.GetCurrentDirectory (), fileName);

            WeTransferClient wt = new WeTransferClient (key);
            await wt.Authorize ();

            PartialFileInfo [] partialFileInfos = new PartialFileInfo [1];
            partialFileInfos [0] = new PartialFileInfo
            {
                Name = "TestFile.png",
                Path = path,
                Size = File.ReadAllBytes (path).Length
            };

            FileTransferResponse ftr = await wt.CreateTransfer (partialFileInfos, "Test Transfer");

            Assert.IsTrue (ftr.Success);

            await wt.Upload (ftr.ID, ftr.Files);

            FileUploadResult result = await wt.FinalizeUpload (ftr.ID, ftr.Files);

            Assert.IsFalse (string.IsNullOrEmpty (result.URL));
        }
    }
}