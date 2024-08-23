using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text.Json;

namespace BackEnd.Pages
{
    public class WorkingWithCatalogsModel : PageModel
    {
        public void OnGet()
        {
            GetRoomsListAsync();
        }
        public async Task<IActionResult> OnPostLinkedRoomInfoAsync(int roomId)
        {
            try
            {
                await GetRoomInfoAsync(roomId);
                var files = FilesStore.AvailableFiles;
                return new JsonResult(new { success = true, files });
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.Error.WriteLine(ex.Message);
                return StatusCode(500, "Error linking room to customer.");
            }
        }
        private async Task GetRoomsListAsync()
        {
            string token = authClass.getAuthToken();
            var requestUri = "https://suleymanovrn.onlyoffice.com/api/2.0/files/rooms";

            var request = WebRequest.Create(requestUri);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + token;

            var response = (HttpWebResponse)await request.GetResponseAsync();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseString = await streamReader.ReadToEndAsync();
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
        }
        private async Task GetRoomInfoAsync(int roomId)
        {
            string token = authClass.getAuthToken();
            var requestUri = $"https://suleymanovrn.onlyoffice.com/api/2.0/files/{roomId}";

            var request = WebRequest.Create(requestUri);
            request.Method = "GET";
            request.Headers["Authorization"] = "Bearer " + token;

            var response = (HttpWebResponse)await request.GetResponseAsync();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseString = await streamReader.ReadToEndAsync();
                var jsonDoc = JsonDocument.Parse(responseString);
                FilesStore.AvailableFiles = jsonDoc.RootElement.GetProperty("response")
                    .GetProperty("files")
                    .EnumerateArray()
                    .Select(file => new FilesStore.AvailableFile
                    {
                        Id = file.GetProperty("id").GetInt32(),
                        Name = file.GetProperty("title").GetString(),
                        WebUrl = file.GetProperty("webUrl").GetString()
                    })
                    .ToList();
            }
        }
    }
}
