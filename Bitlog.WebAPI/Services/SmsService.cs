using Bitlog.WebApi.Entities;
using System.Reflection;
using System.Text.Json;

namespace Bitlog.WebAPI.Services
{
    public class SmsService : IUserNotificationService
    {
        private static string StorageLocation = Environment.CurrentDirectory + "/sms";
        private static JsonSerializerOptions jso = new JsonSerializerOptions
        {
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public void Notify(User user)
        {
            if (Directory.Exists(StorageLocation) is false)
            {
                Directory.CreateDirectory(StorageLocation);
            }
            // just mocking
            File.WriteAllText($"{StorageLocation}/{Guid.NewGuid()}", JsonSerializer.Serialize(user, jso));
        }
    }
}
