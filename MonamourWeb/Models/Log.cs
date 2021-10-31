using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace MonamourWeb.Models
{
    public class Log
    {
        public int Id { get; set; }

        [Required]
        [DisplayName("Дата")]
        public DateTime Date { get; set; }
        
        [Required]
        [DisplayName("Сообщение")]
        public string Message { get; set; }
        
        [Required]
        public int UserId { get; set; }

        [DisplayName("Пользователь")]
        public User User { get; set; }
    }
}