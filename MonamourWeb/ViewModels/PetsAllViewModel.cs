using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class PetsAllViewModel : BaseViewModel<Pet>
    {
        public IEnumerable<PetTag> Tags { get; set; }
        public int? TagId { get; set; }

        public PetsAllViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}