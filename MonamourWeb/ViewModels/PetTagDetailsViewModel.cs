using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class PetTagDetailsViewModel : BaseViewModel<Pet>
    {
        public PetTag PetTag { get; set; }
        
        public PetTagDetailsViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}