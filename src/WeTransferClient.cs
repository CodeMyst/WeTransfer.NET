using System;
using System.IO;
using System.Text;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace WeTransfer.NET
{
    public class WeTransferClient
    {
        private HttpClient httpClient;

        public WeTransferClient (string apiKey)
        {
            httpClient = new HttpClient ();
            httpClient.BaseAddress = new Uri ("https://dev.wetransfer.com/v2/");
            httpClient.DefaultRequestHeaders.Accept.Add (new MediaTypeWithQualityHeaderValue ("application/json"));
            httpClient.DefaultRequestHeaders.Add("x-api-key", apiKey);
        }

        public async Task Authorize ()
        {
            httpClient.DefaultRequestHeaders.Connection.Clear ();
            httpClient.DefaultRequestHeaders.Connection.Add ("POST");
            
            HttpResponseMessage response = await httpClient.PostAsync ("authorize", null);
            response.EnsureSuccessStatusCode ();

            JObject json = JObject.Parse (await response.Content.ReadAsStringAsync ());

            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue ("Bearer", json ["token"].ToString ());
        }

        public async Task<FileTransferResponse> CreateTransfer (PartialFileInfo [] files, string message)
        {
            FileTransfer ft = new FileTransfer
            {
                Message = message,
                Files = files
            };

            string json = JsonConvert.SerializeObject (ft);

            StringContent content = new StringContent (json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await httpClient.PostAsync ("transfers", content);

            FileTransferResponse ftr = JsonConvert.DeserializeObject<FileTransferResponse> (await response.Content.ReadAsStringAsync ());

            for (int i = 0; i < files.Length; i++)
                ftr.Files [i].Path = files [i].Path;

            return ftr;
        }

        public async Task Upload (string transferID, FileInfo [] files)
        {
            Dictionary<FileInfo, string []> urls = new Dictionary<FileInfo, string []> ();

            foreach (FileInfo file in files)
            {
                urls.Add (file, new string [file.Multipart.PartNumbers]);

                for (int i = 1; i <= file.Multipart.PartNumbers; i++)
                {
                    httpClient.DefaultRequestHeaders.Connection.Clear ();
                    httpClient.DefaultRequestHeaders.Connection.Add ("GET");

                    HttpResponseMessage response = await httpClient.GetAsync ($"transfers/{transferID}/files/{file.ID}/upload-url/{i}");

                    urls [file] [i - 1] = JObject.Parse (await response.Content.ReadAsStringAsync ()) ["url"].ToString ();
                }
            }

            HttpClient signedHttpClient = new HttpClient ();

            foreach (KeyValuePair<FileInfo, string []> file in urls)
            {
                if (file.Value.Length == 1)
                {
                    ByteArrayContent byteArrayContent = new ByteArrayContent (File.ReadAllBytes (file.Key.Path));
                    await signedHttpClient.PutAsync (file.Value [0], byteArrayContent);
                }
                else
                {
                    byte [] byteArray = File.ReadAllBytes (file.Key.Path);
                    int previousOffset = 0;
                    int offset = 0;

                    for (int i = 1; i <= file.Key.Multipart.PartNumbers; i++)
                    {
                        int count = 0;

                        if (offset + file.Key.Multipart.ChunkSize > byteArray.Length)
                        {
                            // Last chunk, use any left over bytes as content
                            count = byteArray.Length - (previousOffset + file.Key.Multipart.ChunkSize);
                        }
                        else
                        {
                            count = file.Key.Multipart.ChunkSize;
                        }

                        ByteArrayContent byteArrayContent = new ByteArrayContent (byteArray, offset, count);
                        previousOffset = offset;
                        offset += file.Key.Multipart.ChunkSize;

                        await signedHttpClient.PutAsync (file.Value [i - 1], byteArrayContent);
                    }
                }
            }
        }

        public async Task<FileUploadResult> FinalizeUpload (string transferID, FileInfo [] files)
        {
            foreach (FileInfo file in files)
            {
                StringContent body = new StringContent ($"{{\"part_numbers\":{file.Multipart.PartNumbers}}}", Encoding.UTF8, "application/json");
                await httpClient.PutAsync ($"transfers/{transferID}/files/{file.ID}/upload-complete", body);
            }

            StringContent stringContent = new StringContent ("", Encoding.UTF8, "application/json");
            HttpResponseMessage msg = await httpClient.PutAsync ($"transfers/{transferID}/finalize", stringContent);

            JObject json = JObject.Parse (await msg.Content.ReadAsStringAsync ());

            FileUploadResult res = new FileUploadResult
            {
                URL = json ["url"].ToString (),
                ExpiresAt = json ["expires_at"].ToObject<DateTime> ()
            };

            return res;
        }
    }
}