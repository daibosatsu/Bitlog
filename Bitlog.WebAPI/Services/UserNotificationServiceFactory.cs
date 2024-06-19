using Bitlog.WebApi.Entities;

namespace Bitlog.WebAPI.Services
{
    public class UserNotificationServiceFactory : IUserNotificationServiceFactory
    {
        private readonly IEnumerable<IUserNotificationService> _services;

        public UserNotificationServiceFactory(IEnumerable<IUserNotificationService> services)
        {
            _services = services;
        }

        public IUserNotificationService GetApplicableService(User user)
        {
            if(!string.IsNullOrEmpty(user.Email))
            {
                return _services.First(x => x is SmtpService);
            }

            if (!string.IsNullOrEmpty(user.PhoneNumber))
            {
                return _services.First(x => x is SmsService);
            }

            throw new Exception("Received invalid user");
        }
    }
}
