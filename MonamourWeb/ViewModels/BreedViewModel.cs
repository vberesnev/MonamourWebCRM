using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class BreedViewModel
    {
        public Breed Breed { get; set; }
        public IEnumerable<Animal> Animals { get; set; }
    }
}