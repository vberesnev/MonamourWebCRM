using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class UserViewModel
    {
        public User User { get; set; }
        public IEnumerable<UserRole> Roles { get; set; }
    }
}