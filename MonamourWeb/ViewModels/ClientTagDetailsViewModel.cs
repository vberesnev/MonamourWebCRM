using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class ClientTagDetailsViewModel : BaseViewModel<Client>
    {
        public ClientTag ClientTag { get; set; }
        
        public ClientTagDetailsViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}