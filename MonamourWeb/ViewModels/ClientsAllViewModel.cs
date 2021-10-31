using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class ClientsAllViewModel : BaseViewModel<Client>
    {
        public IEnumerable<ClientTag> Tags { get; set; }
        public int? TagId { get; set; }

        public ClientsAllViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}