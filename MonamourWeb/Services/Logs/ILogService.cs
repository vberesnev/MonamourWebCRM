using System.Threading.Tasks;

namespace MonamourWeb.Services.Logs
{
    public interface ILogService
    {
        Task AddCreationLogAsync<T>(T createdObject, int userId);
    }
}