using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Web;
using System.Net;
using System.Text;
using System.IO;

namespace BackEnd.Pages
{
    public class UploadLargeFilesModel : PageModel
    {
        public void OnGet()
        {
            OnGetFolderInfoAsync("86190");
        }

        public async Task OnGetFolderInfoAsync(string folderId)
        {
            string token = authClass.getAuthToken(); // Получаем токен авторизации
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["folderId"] = folderId;

            var uriBuilder = new UriBuilder($"https://suleymanovrn.onlyoffice.com/api/2.0/files/{folderId}")
            {
                Query = queryParams.ToString()
            };

            var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            using (var response = (HttpWebResponse)request.GetResponse())
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                string responseString = streamReader.ReadToEnd();
                FilesStore.AvailableFiles = JsonDocument.Parse(responseString).RootElement
                    .GetProperty("response")
                    .GetProperty("files")
                    .EnumerateArray()
                    .Select(file => new FilesStore.AvailableFile
                    {
                        Id = file.GetProperty("id").GetInt32(),
                        Name = file.GetProperty("title").GetString()
                    })
                    .ToList();
            }
        }
        public async Task<IActionResult> OnPostUploadFileAsync(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                await AttachFileAsync(file, 86190);
                // В случае ошибки, остаемся на текущей странице
            }
            return RedirectToPage();
        }
        private async Task AttachFileAsync(IFormFile file, int folderId)
        {
            try
            {
                string token = authClass.getAuthToken(); // Получаем токен авторизации
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

                string sessionUrl = $"https://suleymanovrn.onlyoffice.com/api/2.0/files/{folderId}/upload/create_session";

                long chunkUploadSize = 1024 * 1024; // Примерно 1MB

                var body = new
                {
                    CreateOn = DateTime.UtcNow.ToString("o"),
                    FileName = file.FileName,
                    FileSize = file.Length,
                    Id = folderId
                };

                var jsonBody = JsonSerializer.Serialize(body);
                var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

                var sessionResponse = await client.PostAsync(sessionUrl, content);
                sessionResponse.EnsureSuccessStatusCode();

                var sessionJson = await sessionResponse.Content.ReadAsStringAsync();
                var session = JsonDocument.Parse(sessionJson).RootElement;

                var location = session.GetProperty("response").GetProperty("data").GetProperty("location").GetString();

                using (var stream = file.OpenReadStream())
                {
                    int chunkNumber = 0;
                    long totalBytesRead = 0;
                    byte[] buffer = new byte[chunkUploadSize];

                    while (totalBytesRead < file.Length)
                    {
                        int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                        totalBytesRead += bytesRead;

                        var formData = new MultipartFormDataContent();
                        var byteArrayContent = new ByteArrayContent(buffer, 0, bytesRead);
                        formData.Add(byteArrayContent, "file", file.FileName);

                        var uploadResponse = await client.PostAsync(location, formData);
                        uploadResponse.EnsureSuccessStatusCode();

                        chunkNumber++;
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error uploading file: {ex.Message}");
            }
        }
    }
}
