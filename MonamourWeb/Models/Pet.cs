using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonamourWeb.Models
{
    public class Pet
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Кличка")]
        public string Name { get; set; }

        [DisplayName("ДР")]
        public string Bday { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Порода должна быть выбрана")]
        public int BreedId { get; set; }

        [DisplayName("Порода")]
        public Breed Breed { get; set; }
        
        [DisplayName("Инфо")]
        public string Info { get; set; }

        [NotMapped]
        public string ShortInfo
        {
            get
            {
                if (string.IsNullOrEmpty(Info))
                    return string.Empty;

                if (Info.Length > 20)
                    return Info.Substring(0, 20) + "...";
                return Info;
            }
        }

        [DisplayName("Жив")]
        public bool Alive { get; set; }

        [DisplayName("Клиенты")]
        public ICollection<Client> Clients { get; set; }
        
        [DisplayName("Теги")]
        public ICollection<PetTag> Tags { get; set; }

        public Pet()
        {
            Clients = new List<Client>();
            Tags = new List<PetTag>();
        }

        public void Update(Pet pet)
        {
            Name = pet.Name;
            Bday = pet.Bday;
            BreedId = pet.BreedId;
            Info = pet.Info;
            Alive = pet.Alive;

            Clients = pet.Clients;
            Tags = pet.Tags;
        }
    }
}