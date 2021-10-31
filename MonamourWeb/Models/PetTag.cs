using System.Collections.Generic;

namespace MonamourWeb.Models
{
    public class PetTag : Tag
    {
        public ICollection<Pet> Pets { get; set; }
        
        public PetTag()
        {
            Pets = new List<Pet>();
        }
    }
}