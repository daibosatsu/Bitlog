using Bitlog.WebApi.Entities;
using System.Net.Mail;
using System.Reflection;
using System.Text.Json;

namespace Bitlog.WebAPI.Services
{
    public class SmtpService : IUserNotificationService
    {
        private static string StorageLocation = Environment.CurrentDirectory + "/smtp";
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

            SmtpClient client = new SmtpClient();
            client.DeliveryMethod = SmtpDeliveryMethod.SpecifiedPickupDirectory;
            client.PickupDirectoryLocation = StorageLocation;
            client.Send(
                "notification@test.com",
                user.Email,
                "Your account has been created",
                JsonSerializer.Serialize(user, jso));
        }
    }
}
