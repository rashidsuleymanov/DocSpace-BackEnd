using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using static BackEnd.RoomStore;
using System.Net;
using System.Text.Json;
using System.Web;

namespace BackEnd.Pages
{
    public class BuilderInDocSpaceModel : PageModel
    {
        public void OnGet()
        {
            OpenEdit();
        }
        private async Task OpenEdit()
        {
            string token = authClass.getAuthToken();
            var queryParams = HttpUtility.ParseQueryString(string.Empty);
            queryParams["type"] = "504442";
            var requestUri = new UriBuilder($"https://suleymanovrn.onlyoffice.com/api/2.0/files/file/{504442}/openedit")
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
            }
        }
    }
}
