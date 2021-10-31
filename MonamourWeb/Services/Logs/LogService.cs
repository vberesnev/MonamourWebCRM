using System;
using System.Reflection;
using System.Text;
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

        public async Task AddCreationLogAsync<T>(T createdObject, int userId)
        {
            var log = new Log()
            {
                Date = DateTime.Now,
                Message = "Добавлен объект " + createdObject.ToString(),
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
                Message = "Удален объект " + deletedObject.ToString(),
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddUpdatedLogAsync<T>(T oldObject, T newObject, int userId)
        {
            throw new NotImplementedException();

            var log = new Log()
            {
                Date = DateTime.Now,
                Message = " объект " + oldObject.ToString(),
                UserId = userId
            };
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

    }
}