using System.ComponentModel.DataAnnotations;

namespace Bitlog.WebApi.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string? Email { get; private set; }
        public string? PhoneNumber { get; private set; }

        [Required]
        public string Password { get; private set; }

        internal static string GetFormattedPhoneNumber(string phoneNumber)
        {
            if (string.IsNullOrEmpty(phoneNumber))
            {
                return phoneNumber;
            }

            if (phoneNumber[0] == '0' && phoneNumber[1] == '0')
            {
                phoneNumber = '+' + phoneNumber.Substring(2);
            }

            if (phoneNumber.Length == 10)
            {
                phoneNumber = "+46" + phoneNumber.Substring(1);
            }

            return phoneNumber;
        }

        public static User CreateUser(string email, string phoneNumber, string password)
        {
            var user = new User
            {
                Email = email,
                Password = password
            };

            phoneNumber = GetFormattedPhoneNumber(phoneNumber);
            if (!string.IsNullOrEmpty(phoneNumber))
            {
                user.PhoneNumber = phoneNumber;
            }

            return user;
        }
    }
}