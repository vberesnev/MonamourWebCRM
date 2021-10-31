using System.Collections.Generic;

namespace MonamourWeb.Models
{
    public class ClientTag : Tag
    {
        public ICollection<Client> Clients { get; set; }
        
        public ClientTag()
        {
            Clients = new List<Client>();
        }
    }
}