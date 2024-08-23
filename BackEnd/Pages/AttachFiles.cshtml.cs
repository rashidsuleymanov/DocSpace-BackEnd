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
    public class AttachFilesModel : PageModel
    {
        public async void OnGet()
        {
            await OnGetFolderInfoAsync("45555");
        }

        public IActionResult OnPostAttachFileToTask(int taskId, int fileId, string fileName)
        {
            var task = TaskStore.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.Attachments.Add(new TaskStore.Attachment
                {
                    FileId = fileId,
                    FileName = fileName,
                    WebUrl = GetFileUrl(fileId).Result
                });
            }
            return Page();
        }

        public IActionResult OnPostCreateFile(string fileName, int taskId)
        {
            CreateFile(fileName, taskId).Wait();
            return Page();
        }

        public async Task OnGetFolderInfoAsync(string folderId)
        {
            string token = authClass.getAuthToken();
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["folderId"] = folderId;

            var uriBuilder = new UriBuilder($"https://suleymanovrashidn.onlyoffice.io/api/2.0/files/{folderId}")
            {
                Query = queryParams.ToString()
            };

            var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            using var response = (HttpWebResponse)request.GetResponse();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

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

        public async Task CreateFile(string fileName, int taskId)
        {
            string token = authClass.getAuthToken();
            string folderId = "45555";
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["folderId"] = folderId;

            var uriBuilder = new UriBuilder($"https://suleymanovrashidn.onlyoffice.io/api/2.0/files/{folderId}/file")
            {
                Query = queryParams.ToString()
            };

            var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            var body = JsonSerializer.Serialize(new { Title = fileName });
            var data = Encoding.UTF8.GetBytes(body);

            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using var response = (HttpWebResponse)request.GetResponse();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            var session = JsonDocument.Parse(responseString).RootElement;
            var id = session.GetProperty("response").GetProperty("id").GetInt32();
            var attachFileName = session.GetProperty("response").GetProperty("title").GetString();
            var webUrl = session.GetProperty("response").GetProperty("webUrl").GetString();

            var task = TaskStore.Tasks.FirstOrDefault(t => t.Id == taskId);
            if (task != null)
            {
                task.Attachments.Add(new TaskStore.Attachment
                {
                    FileId = id,
                    FileName = attachFileName,
                    WebUrl = webUrl
                });
            }
            else
            {
                throw new Exception("Task not found.");
            }
        }

        public async Task<string> GetFileUrl(int fileId)
        {
            string token = authClass.getAuthToken();
            var request = WebRequest.Create($"https://suleymanovrn.onlyoffice.com/api/2.0/files/file/{fileId}/openedit");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            using var response = (HttpWebResponse)request.GetResponse();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            var session = JsonDocument.Parse(responseString).RootElement;
            return session.GetProperty("response").GetProperty("file").GetProperty("webUrl").GetString();
        }
    }
}
