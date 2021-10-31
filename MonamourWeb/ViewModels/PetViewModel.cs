using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class PetViewModel
    {
        public Pet Pet { get; set; }
        public List<PetTag> AllTags { get; set; }

        public PetViewModel()
        {
            Pet = new Pet();
            AllTags = new List<PetTag>();
        }
    }
}