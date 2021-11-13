using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class BreedsAllViewModel : BaseViewModel<Breed>
    {
        public IEnumerable<Animal> Animals { get; set; }
        public int? AnimalId { get; set; }

        public BreedsAllViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}