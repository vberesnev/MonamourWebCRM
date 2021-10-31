using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class LoginViewModel
    {
        public IEnumerable<string> Logins { get; set; }
        public User User { get; set; }
    }
}