using System;
using System.Threading.Tasks;
using MonamourWeb.Models;

namespace MonamourWeb.Services.Logs
{
    public class LogService : ILogService
    {
        private readonly MonamourDataBaseContext _context;

        public LogService(MonamourDataBaseContext context)
        {
            _context = context;
        }

        public async Task AddLoginLogAsync(string userName, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "ПОЛЬЗОВАТЕЛЬ "+ userName +" ВОШЕЛ В СИСТЕМУ",
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddLogoutLogAsync(string userName, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "ПОЛЬЗОВАТЕЛЬ "+ userName +" ВЫШЕЛ ИЗ СИСТЕМЫ",
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddCreationLogAsync<T>(T createdObject, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "ДОБАВЛЕН ОБЪЕКТ " + createdObject,
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddDeletedLogAsync<T>(T deletedObject, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "УДАЛЕН ОБЪЕКТ " + deletedObject,
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddUpdatedLogAsync<T>(T oldObject, T newObject, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "ОБНОВЛЕН ОБЪЕКТ " + oldObject + " НА НОВЫЙ ОБЪЕКТ " + newObject,
                UserId = userId
            };
                
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddBlockUserLogAsync(string userName, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "ПОЛЬЗОВАТЕЛЬ "+ userName +" ЗАБЛОКИРОВАН",
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddUnblockUserLogAsync(string userName, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "ПОЛЬЗОВАТЕЛЬ "+ userName +" РАЗБЛОКИРОВАН",
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }
    }
}