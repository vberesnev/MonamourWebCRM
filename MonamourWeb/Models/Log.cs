using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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

        [NotMapped]
        public string ShortMessage
        {
            get
            {
                if (string.IsNullOrEmpty(Message))
                    return string.Empty;

                if (Message.Length > 100)
                    return Message.Substring(0, 20) + "...";
                return Message;
            }
        }
        
        [Required]
        public int UserId { get; set; }

        [DisplayName("Пользователь")]
        public User User { get; set; }
    }
}