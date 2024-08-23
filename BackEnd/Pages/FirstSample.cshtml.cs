using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Web;

namespace BackEnd.Pages
{
    public class FirstSampleModel : PageModel
    {
        [BindProperty]
        public string login { get; set; }

        [BindProperty]
        public string password { get; set; }
        [BindProperty]
        public string confirmPassword { get; set; }
        public void SendInvate(string email)
        {

        }
          
        public void OnGet()
        {
        
        }
        public IActionResult OnPostLogin()
        {
            string url = "https://suleymanovrn.onlyoffice.com/api/2.0/people/invite";

            var request = WebRequest.Create(url);
            request.Method = "POST";
            request.ContentType = "application/json";

            // Добавьте токен в заголовок Authorization
            var token = authClass.getAuthToken();
            request.Headers["Authorization"] = "Bearer " + token;

            var body = "{\"Email\":\"suleymaovrn@mail.ru\"}";
            var data = Encoding.UTF8.GetBytes(body);

            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }

            var response = (HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            return Page();
        }




        //public void OnGet()
        //{
        //    LoadItems();
        //}
        //private void LoadItems()
        //{
        //    string token = authClass.getAuthToken();
        //    // Создание параметров запроса
        //    var queryParams = HttpUtility.ParseQueryString(string.Empty);

        //    // Создание URL с параметрами
        //    var uriBuilder = new UriBuilder("https://suleymanovrn.onlyoffice.com/api/2.0/files/86190")
        //    {
        //        Query = queryParams.ToString()
        //    };

        //    var request1 = (HttpWebRequest)WebRequest.Create(uriBuilder.Uri);
        //    request1.Method = "GET";
        //    request1.ContentType = "application/json";
        //    request1.Headers["Authorization"] = "Bearer " + token;

        //    var response1 = (HttpWebResponse)request1.GetResponse();
        //    var responseString1 = new StreamReader(response1.GetResponseStream()).ReadToEnd();
        //    var root = JsonSerializer.Deserialize<Root>(responseString1);
        //    Items = root.response.files;
        //}
        //public IActionResult OnPost()
        //{
        //    LoadItems();
        //    // Логика для обработки выбранного элемента
        //    var selectedItem = Items.Find(item => item.id == SelectedItemId);

        //    if (selectedItem != null)
        //    {
        //        // Делаем что-то с выбранным элементом
        //        // Например, перенаправляем на другую страницу
        //        // return RedirectToPage("AnotherPage", new { id = selectedItem.Id });
        //    }

        //    // Просто возвращаем ту же страницу для демонстрации
        //    return Page();
        //}
    }
}
