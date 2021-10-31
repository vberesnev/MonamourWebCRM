using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class BreedsAllViewModel : BaseViewModel<Breed>
    {
        public BreedsAllViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}