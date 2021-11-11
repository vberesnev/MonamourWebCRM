using System.Threading.Tasks;

namespace MonamourWeb.Services.Logs
{
    public interface ILogService
    {
        Task AddLoginLogAsync(string userName, int userId);
        Task AddLogoutLogAsync(string userName, int userId);
        Task AddCreationLogAsync<T>(T createdObject, int userId);
        Task AddDeletedLogAsync<T>(T deletedObject, int userId);
        Task AddUpdatedLogAsync<T>(T oldObject, T newObject, int userId);
        Task AddBlockUserLogAsync(string userName, int userId);
        Task AddUnblockUserLogAsync(string userName, int userId);
        Task AddChangePasswordLog(string userName, int userId);
    }
}