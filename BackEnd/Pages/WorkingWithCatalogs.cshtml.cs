using BackEnd.Classes;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace BackEnd.Pages
{
    public class WorkingWithCatalogsModel : PageModel
    {
        public async void OnGet()
        {
            await GetRoomsListAsync();
        }

        public async Task<IActionResult> OnGetRoomInfoAsync(int roomId)
        {
            try
            {
                await GetRoomInfoAsync(roomId);
                return new JsonResult(new { success = true, files = FilesStore.AvailableFiles });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, "Test.");
            }
        }

        public async Task<IActionResult> OnGetCopyFileAsync(int fileId, int destFolderId)
        {
            try
            {
                await CopyFileAsync(fileId, destFolderId);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        private async Task CopyFileAsync(int fileId, int folderId)
        {
            string token = authClass.getAuthToken();

            var uri = new Uri($"https://suleymanovrn.onlyoffice.com/api/2.0/files/file/{fileId}/copyas");
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = $"Bearer {token}";

            var body = JsonSerializer.Serialize(new
            {
                destFolderId = folderId,
                destTitle = await GetFileInfo(fileId),
                EnableExternalExt = true,
                Password = ""
            });

            var data = Encoding.UTF8.GetBytes(body);
            request.ContentLength = data.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error response from API: {responseString}");
            }
        }

        public async Task<IActionResult> OnGetMoveFileAsync(int fileId, int folderId, int destFolderId)
        {
            try
            {
                await MoveFileAsync(fileId, folderId, destFolderId);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        private async Task MoveFileAsync(int fileId, int folderId, int destFolderId)
        {
            string token = authClass.getAuthToken();

            var uri = new Uri("https://suleymanovrn.onlyoffice.com/api/2.0/files/fileops/move");
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = $"Bearer {token}";

            var body = JsonSerializer.Serialize(new
            {
                folderIds = folderId,
                fileIds = fileId,
                destFolderId = destFolderId
            });

            var data = Encoding.UTF8.GetBytes(body);
            request.ContentLength = data.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error response from API: {responseString}");
            }
        }

        public async Task<IActionResult> OnGetCreateFolderAsync(string folderTitle, int folderId)
        {
            try
            {
                await CreateFolderAsync(folderTitle, folderId);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        private async Task CreateFolderAsync(string folderTitle, int folderId)
        {
            string token = authClass.getAuthToken();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["folderId"] = folderId.ToString();

            var uriBuilder = new UriBuilder($"https://suleymanovrn.onlyoffice.com/api/2.0/files/folder/{folderId}")
            {
                Query = queryParams.ToString()
            };

            var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = $"Bearer {token}";

            var body = JsonSerializer.Serialize(new { title = folderTitle });
            var data = Encoding.UTF8.GetBytes(body);
            request.ContentLength = data.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error response from API: {responseString}");
            }
        }

        public async Task<IActionResult> OnGetRenameFolderAsync(string folderTitle, int folderId)
        {
            try
            {
                await RenameFolderAsync(folderTitle, folderId);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        private async Task RenameFolderAsync(string folderTitle, int folderId)
        {
            string token = authClass.getAuthToken();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["folderId"] = folderId.ToString();

            var uriBuilder = new UriBuilder($"https://suleymanovrn.onlyoffice.com/api/2.0/files/folder/{folderId}")
            {
                Query = queryParams.ToString()
            };

            var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = $"Bearer {token}";

            var body = JsonSerializer.Serialize(new { title = folderTitle });
            var data = Encoding.UTF8.GetBytes(body);
            request.ContentLength = data.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error response from API: {responseString}");
            }
        }

        public async Task<IActionResult> OnGetDeleteFolderAsync( int folderId)
        {
            try
            {
                await DeleteFolderAsync(folderId);
                return new JsonResult(new { success = true });
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, "Internal Server Error.");
            }
        }

        private async Task DeleteFolderAsync(int folderId)
        {
            string token = authClass.getAuthToken();

            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["folderId"] = folderId.ToString();

            var uriBuilder = new UriBuilder($"https://suleymanovrn.onlyoffice.com/api/2.0/files/folder/{folderId}")
            {
                Query = queryParams.ToString()
            };

            var request = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
            request.Method = "DELETE";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = $"Bearer {token}";

            var body = JsonSerializer.Serialize(new { deleteAfter = true, immediately = false});
            var data = Encoding.UTF8.GetBytes(body);
            request.ContentLength = data.Length;

            using (var stream = await request.GetRequestStreamAsync())
            {
                await stream.WriteAsync(data, 0, data.Length);
            }

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            if (response.StatusCode != HttpStatusCode.OK)
            {
                throw new Exception($"Error response from API: {responseString}");
            }
        }

        public async Task<string> GetFileInfo(int fileId)
        {
            string token = authClass.getAuthToken();
            var request = WebRequest.Create($"https://suleymanovrn.onlyoffice.com/api/2.0/files/file/{fileId}");
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            var session = JsonDocument.Parse(responseString).RootElement;
            return session.GetProperty("response").GetProperty("title").GetString();
        }

        private async Task GetRoomsListAsync()
        {
            string token = authClass.getAuthToken();
            var requestUri = "https://suleymanovrn.onlyoffice.com/api/2.0/files/rooms";

            var request = WebRequest.Create(requestUri);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + token;

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            var jsonDoc = JsonDocument.Parse(responseString);
            RoomStore.RoomsList = jsonDoc.RootElement.GetProperty("response")
                .GetProperty("folders")
                .EnumerateArray()
                .Select(room => new RoomStore.Room
                {
                    Id = room.GetProperty("id").GetInt32(),
                    Name = room.GetProperty("title").GetString()
                })
                .ToList();
        }

        private async Task GetRoomInfoAsync(int roomId)
        {
            string token = authClass.getAuthToken();
            var requestUri = $"https://suleymanovrn.onlyoffice.com/api/2.0/files/{roomId}";

            var request = WebRequest.Create(requestUri);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + token;

            using var response = (HttpWebResponse)await request.GetResponseAsync();
            using var streamReader = new StreamReader(response.GetResponseStream());
            string responseString = await streamReader.ReadToEndAsync();

            var jsonDoc = JsonDocument.Parse(responseString);
            var icons = new Icons();

            FilesStore.AvailableFiles = jsonDoc.RootElement.GetProperty("response")
                .GetProperty("folders")
                .EnumerateArray()
                .Select(folder => new FilesStore.AvailableFile
                {
                    Id = folder.GetProperty("id").GetInt32(),
                    Name = folder.GetProperty("title").GetString(),
                    Icon = icons.GetIcon("Folder")
                })
                .ToList();

            FilesStore.AvailableFiles.AddRange(jsonDoc.RootElement.GetProperty("response")
                .GetProperty("files")
                .EnumerateArray()
                .Select(file => new FilesStore.AvailableFile
                {
                    Id = file.GetProperty("id").GetInt32(),
                    Name = file.GetProperty("title").GetString(),
                    WebUrl = file.GetProperty("webUrl").GetString(),
                    Icon = icons.GetIcon(file.GetProperty("title").GetString())
                })
                .ToList());
        }
    }
}
