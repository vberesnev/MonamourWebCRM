using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class ClientViewModel
    {
        public Client Client { get; set; }
        public List<ClientTag> AllTags { get; set; }

        public ClientViewModel()
        {
            Client = new Client();
            AllTags = new List<ClientTag>();
        }
    }
}