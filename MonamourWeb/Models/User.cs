using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Логин")]
        public string Name { get; set; }
        
        [Required]
        [DisplayName("Пароль")]
        public string Password { get; set; }
        
        [DisplayName("Роль")]
        public UserRole Role { get; set; }
        
        [Required]
        public int RoleId { get; set; }

        [DisplayName("Заблокирован")]
        public bool Blocked { get; set; }

        public override string ToString()
        {
            return $"Пользователь: [Id]: {Id}; [Логин]: {Name}; [Id роли]: {RoleId}: [Заблокирован]: {Helper.BoolToStringConverter(Blocked)}";
        }
    }
}