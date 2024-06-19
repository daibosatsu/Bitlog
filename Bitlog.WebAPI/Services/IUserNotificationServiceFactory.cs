using Bitlog.WebApi.Entities;

namespace Bitlog.WebAPI.Services
{
    public interface IUserNotificationServiceFactory
    {
        IUserNotificationService GetApplicableService(User user);
    }
}
