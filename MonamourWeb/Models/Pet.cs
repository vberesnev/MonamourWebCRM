using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MonamourWeb.Models
{
    public class Pet : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "У питомца должна быть кличка")]
        [DisplayName("Кличка")]
        public string Name { get; set; }

        [DisplayName("ДР")]
        public string Bday { get; set; }

        [Required(ErrorMessage = "У питомца должна быть порода")]
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
            Breed = pet.Breed;
            Info = pet.Info;
            Alive = pet.Alive;

            Clients = pet.Clients;
            Tags = pet.Tags;
        }

        public override string ToString()
        {
            var clients = string.Join(" ", Clients.Select(x => x.Name + " (" + x.Phone + ") [" + x.Id + "]"));
            var tags = string.Join(" ", Tags.Select(x => x.Title + " [" + x.Id + "]"));

            return $"Питомец: [Id]: {Id}; [Кличка]: {Name}; [Id породы]: {BreedId}; [Порода]: {Breed?.Title}; [Инфо]: {Info}; [ДР]: {Bday}; [Жив]: {Alive}; [Клиенты]: {clients}; [Тэги]: {tags};";
        }

        public object Clone()
        {
            return new Pet()
            {
                Id = this.Id,
                Name = this.Name,
                Bday = this.Bday,
                BreedId = this.BreedId,
                Breed = this.Breed?.Clone() as Breed,
                Info = this.Info,
                Alive = this.Alive,
                Clients = this.Clients,
                Tags = this.Tags
            };
        }
    }
}