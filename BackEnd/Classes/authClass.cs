using BackEnd.Pages;
using System.Net;
using System.Text.Json.Serialization;
using System.Text.Json;
using System.Web;
using Microsoft.AspNetCore.Mvc;

namespace BackEnd
{
    public static class authClass
    {
        private class Response
        {
            [JsonPropertyName("token")]
            public string Token { get; set; }

            [JsonPropertyName("expires")]
            public DateTime Expires { get; set; }

            [JsonPropertyName("sms")]
            public bool Sms { get; set; }

            [JsonPropertyName("tfa")]
            public bool Tfa { get; set; }
        }

        private class RootObject
        {
            [JsonPropertyName("response")]
            public Response Response { get; set; }
        }
        public static string getAuthToken()
        {
            var request = System.Net.WebRequest.Create("https://suleymanovrn.onlyoffice.com/api/2.0/authentication");
            request.Method = "POST";
            request.ContentType = "application/json";

            var body = "{\"UserName\":\"suleymanovrashidn@gmail.com\",\"Password\":\"#9PasportNP#\"}";
            var data = System.Text.Encoding.UTF8.GetBytes(body);

            request.ContentLength = data.Length;
            using (var stream = request.GetRequestStream())
            {
                stream.Write(data, 0, data.Length);
            }
            var response = (System.Net.HttpWebResponse)request.GetResponse();
            var responseString = new StreamReader(response.GetResponseStream()).ReadToEnd();
            RootObject root = JsonSerializer.Deserialize<RootObject>(responseString);
            return root.Response.Token;
        }



        //public async Task<IActionResult> OnPostAttachFileToTaskAsync(int taskId, [FromBody] FileModel file)
        //{
        //    try
        //    {
        //        // Пример: сохранение информации о прикрепленном файле в базе данных или файловой системе
        //        var attachment = new Attachments
        //        {
        //            TaskId = taskId,
        //            FileName = file.FileName, // Используем свойство FileName из модели FileModel
        //            FileId = file.Id // Используем свойство FileId из модели FileModel
        //        };

        //        AttachmentsList.Add(attachment); // Добавление прикрепленного файла в список

        //        // Здесь может быть логика сохранения attachment в базу данных или файловую систему

        //        return new JsonResult(new { success = true });
        //    }
        //    catch (Exception ex)
        //    {
        //        return new JsonResult(new { success = false, error = ex.Message });
        //    }
        //}

        //public async Task<IActionResult> OnPostUploadFileAsync(IFormFile file)
        //{
        //    if (file != null && file.Length > 0)
        //    {
        //        await AttachFileAsync(file, 86190);
        //        // В случае ошибки, остаемся на текущей странице
        //    }
        //    return Page();
        //}

        //public async Task<IActionResult> OnPostCreateFile(string fileName)
        //{
        //    if (!string.IsNullOrEmpty(fileName))
        //    {
        //        try
        //        {
        //            await CreateFileAsync(fileName); // Создание файла на сервере OnlyOffice
        //            return Page(); // Возвращаем текущую страницу
        //        }
        //        catch (Exception ex)
        //        {
        //            return BadRequest(new { error = $"Failed to create file: {ex.Message}" });
        //        }
        //    }

        //    return Page(); // Возвращаем текущую страницу
        //}



        //public async Task GetActiveOperationsAsync()
        //{
        //    string token = authClass.getAuthToken(); // Получаем токен авторизации

        //    var request = (HttpWebRequest)WebRequest.Create("https://suleymanovrn.onlyoffice.com/api/2.0/files/fileops");
        //    request.Method = "GET";
        //    request.ContentType = "application/json";
        //    request.Headers["Authorization"] = "Bearer " + token;

        //    using (var response = (HttpWebResponse)request.GetResponse())
        //    using (var streamReader = new StreamReader(response.GetResponseStream()))
        //    {
        //        string responseString = streamReader.ReadToEnd();
        //        ActiveOperations = JsonDocument.Parse(responseString).RootElement
        //            .GetProperty("response")
        //            .GetProperty("fileops")
        //            .EnumerateArray()
        //            .Select(op => new FileOperation
        //            {
        //                Id = op.GetProperty("id").GetInt32(),
        //                Status = op.GetProperty("status").GetString(),
        //                FileName = op.GetProperty("fileName").GetString()
        //            })
        //            .ToList();
        //    }
        //}

        //public async Task<IActionResult> OnPostTerminateOperationAsync(int operationId)
        //{
        //    string token = authClass.getAuthToken(); // Получаем токен авторизации

        //    var request = (HttpWebRequest)WebRequest.Create($"https://suleymanovrn.onlyoffice.com/api/2.0/files/fileops/terminate/{operationId}");
        //    request.Method = "PUT";
        //    request.ContentType = "application/json";
        //    request.Headers["Authorization"] = "Bearer " + token;

        //    using (var response = (HttpWebResponse)request.GetResponse())
        //    using (var streamReader = new StreamReader(response.GetResponseStream()))
        //    {
        //        string responseString = streamReader.ReadToEnd();
        //    }

        //    // Обновляем список активных операций после завершения
        //    await GetActiveOperationsAsync();

        //    return RedirectToPage();
        //}
    }
}
