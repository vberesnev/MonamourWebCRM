using System;
using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class LogsViewModel : BaseViewModel<Log>
    {
        public DateTime Begin { get; set; }
        public DateTime End { get; set; }
        public IEnumerable<User> Users { get; set; }
        public int? UserId { get; set; }

        public LogsViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}