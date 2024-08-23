using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using System.Linq;
using System.Net;
using static BackEnd.FileSystem;
using System.Web;
using System;
using static BackEnd.CustomerStore;
using System.Net.Mail;

namespace BackEnd.Pages
{
    public class WorkingWithRoomsModel : PageModel
    {
        public async Task OnGetAsync()
        {
            await GetRoomsListAsync();
        }

        public async Task<IActionResult> OnPostCreateRoomAsync(string roomName, int customerId, int typeRoom)
        {
            var customer = CustomerStore.Customers.FirstOrDefault(t => t.Id == customerId);
            if (customer != null)
            {
                try
                {
                    int roomId = await CreateRoomAsync(roomName, typeRoom);
                    customer.RoomId = roomId;
                    customer.RoomName = roomName;
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                    Console.Error.WriteLine(ex.Message);
                    return StatusCode(500, "Error creating room.");
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnPostLinkRoomAsync(string roomName, int customerId,int roomId)
        {
            var customer = CustomerStore.Customers.FirstOrDefault(t => t.Id == customerId);
            if (customer != null)
            {
                try
                {
                    customer.RoomId = roomId;
                    customer.RoomName = roomName;
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                    Console.Error.WriteLine(ex.Message);
                    return StatusCode(500, "Error creating room.");
                }
            }
            return Page();
        }
        public async Task<IActionResult> OnGetLinkedRoomInfoAsync(int customerId)
        {
            try
            {
                await GetRoomInfoAsync(customerId);
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

        public async Task<IActionResult> OnPostCloseDealAsync(int dealId)
        {
            var customer = CustomerStore.Customers.FirstOrDefault(t => t.Id == dealId);
            if (customer != null)
            {
                try
                {
                    await CloseDealAsync(customer.RoomId);
                }
                catch (Exception ex)
                {
                    // Логирование ошибки
                    Console.Error.WriteLine(ex.Message);
                    return StatusCode(500, "Error closing deal.");
                }
            }
            return Page();
        }

        public IActionResult OnPostLinkRoomToCustomer(int customerId, int roomId, string roomName)
        {
            var customer = CustomerStore.Customers.FirstOrDefault(t => t.Id == customerId);
            if (customer != null)
            {
                customer.RoomName = roomName;
                customer.RoomId = roomId;
            }
            return Page();
        }
        public async Task<IActionResult> OnPostInviteUserToRoomAsync(int inviteCustomerId, string email)
        {
            var customer = CustomerStore.Customers.FirstOrDefault(t => t.Id == inviteCustomerId);
            if (customer != null)
            {
                var invitationLink = await GetInviteLink(customer.RoomId);
                SendInviteLink("your_email@example.com", invitationLink);
            }
            return Page();
        }
        private async Task SendInviteLink(string email, string invitationLink)
        {
            SmtpClient smtpClient = new SmtpClient("smtp.example.com")
            {
                Port = 587, 
                Credentials = new NetworkCredential("your_email@example.com", "your_password"),
                EnableSsl = true,
            };

            MailMessage mailMessage = new MailMessage
            {
                From = new MailAddress("your_email@example.com"),
                Subject = "Invitation to Collaborate on a Document",
                Body = $"You have been invited to work together on the documents {invitationLink}",
                IsBodyHtml = false,
            };

            mailMessage.To.Add("recipient@example.com");

            smtpClient.Send(mailMessage);
        }

        private async Task<string> GetInviteLink(int roomId)
        {
            string token = authClass.getAuthToken();
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["type"] = "0";
            var requestUri = new UriBuilder($"https://suleymanovrn.onlyoffice.com/api/2.0/files/rooms/{roomId}/links")
            {
                Query = queryParams.ToString()
            };

            var request = (HttpWebRequest)WebRequest.Create(requestUri.Uri);
            request.Method = "GET";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            var response = (HttpWebResponse)await request.GetResponseAsync();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseString = await streamReader.ReadToEndAsync();
                var jsonDoc = JsonDocument.Parse(responseString);
                return jsonDoc.RootElement.GetProperty("response").GetProperty("sharedTo")
                             .GetProperty("shareLink")
                             .GetString();
            }
        }
        private async Task<int> CreateRoomAsync(string roomName, int typeRoom)
        {
            string token = authClass.getAuthToken();
            var requestUri = "https://suleymanovrn.onlyoffice.com/api/2.0/files/rooms";
            var body = new { Title = roomName, RoomType = typeRoom };
            var jsonBody = JsonSerializer.Serialize(body);

            var request = WebRequest.Create(requestUri);
            request.Method = "POST";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonBody);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)await request.GetResponseAsync();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var responseString = await streamReader.ReadToEndAsync();
                var jsonDoc = JsonDocument.Parse(responseString);
                return jsonDoc.RootElement.GetProperty("response").GetProperty("id").GetInt32();
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


        private async Task CloseDealAsync(int roomId)
        {
            string token = authClass.getAuthToken();
            var requestUri = $"https://suleymanovrn.onlyoffice.com/api/2.0/files/rooms/{roomId}/archive";
            var body = new { DeleteAfter = true };
            var jsonBody = JsonSerializer.Serialize(body);

            var request = WebRequest.Create(requestUri);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Headers["Authorization"] = "Bearer " + token;

            using (var streamWriter = new StreamWriter(request.GetRequestStream()))
            {
                streamWriter.Write(jsonBody);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var response = (HttpWebResponse)await request.GetResponseAsync();
            if (!response.StatusCode.Equals(HttpStatusCode.OK))
            {
                using (var streamReader = new StreamReader(response.GetResponseStream()))
                {
                    var errorMessage = await streamReader.ReadToEndAsync();
                    throw new HttpRequestException($"Error closing deal: {response.StatusCode} - {errorMessage}");
                }
            }
        }

        private async Task GetRoomInfoAsync(int customerId)
        {
            string roomId = CustomerStore.Customers.FirstOrDefault(t => t.Id == customerId).RoomId.ToString();
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
