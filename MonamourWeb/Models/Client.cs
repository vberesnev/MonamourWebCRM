using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MonamourWeb.Models
{
    public class Client : ICloneable
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "У клиента должно быть имя")]
        [DisplayName("Имя")]
        public string Name { get; set; }

        [DisplayName("Телефон")]
        public string Phone { get; set; }

        [NotMapped]
        public string SecretPhone
        {
            get
            {
                if (string.IsNullOrEmpty(Phone))
                    return string.Empty;

                var phones = Phone.Split(" ");
                var secretPhone = string.Empty;
                
                for (var i = 0; i < phones.Length; i++)
                {
                    var phone = phones[i];
                    if (phone.Length < 5)
                    {
                        secretPhone += Phone;
                    }
                    else
                    {
                        secretPhone += new string('*', phone.Length - 4);
                        secretPhone += phone.Substring(phone.Length - 4);
                    }

                    if (i + 1 != phones.Length)
                        secretPhone += "\r\n";
                }

                return secretPhone;
            }
        }

        [DisplayName("E-mail")]
        public string Email { get; set; }

        [DisplayName("Instagram")]
        public string Instagram { get; set; }

        [DisplayName("Соц. сети")]
        public string Social { get; set; }

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

        [DisplayName("Бонусы")]
        public int Bonus { get; set; }

        [DisplayName("Питомцы")]
        public ICollection<Pet> Pets { get; set; }
        
        [DisplayName("Теги")]
        public ICollection<ClientTag> Tags { get; set; }

        public Client()
        {
            Pets = new List<Pet>();
            Tags = new List<ClientTag>();
        }

        public void Update(Client client)
        {
            Name = client.Name?.Trim();
            Phone = client.Phone;
            Email = client.Email;
            Instagram = client.Instagram;
            Social = client.Social;
            Info = client.Info;
            Bonus = client.Bonus;

            Pets = client.Pets;
            Tags = client.Tags;
        }

        public override string ToString()
        {
            var pets = string.Join(" ", Pets.Select(x => x.Name + " (" + x.Breed?.Title + ") [" + x.Id + "]"));
            var tags = string.Join(" ", Tags.Select(x => x.Title + " [" + x.Id + "]"));

            return $"Клиент: [Id]: {Id}; [Имя]: {Name}; [Телефон]: {Phone}; [E-mail]: {Email}; [Instagram]: {Instagram}; [Соц. сети]: {Social};" +
                   $" [Инфо]: {Info}; [Бонусы]: {Bonus}; [Питомцы]: {pets}; [Тэги]: {tags};";
        }

        public object Clone()
        {
            return new Client()
            {
                Id = this.Id,
                Name = this.Name,
                Phone = this.Phone,
                Email = this.Email,
                Instagram = this.Instagram,
                Social = this.Social,
                Info = this.Info,
                Bonus = this.Bonus,
                Pets = this.Pets,
                Tags = this.Tags
            };
        }
    }
}