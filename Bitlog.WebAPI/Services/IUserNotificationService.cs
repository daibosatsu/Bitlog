using Bitlog.WebApi.Entities;

namespace Bitlog.WebAPI.Services
{
    public interface IUserNotificationService
    {
        void Notify(User user);
    }
}
