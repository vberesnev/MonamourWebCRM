using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MonamourWeb.Models
{
    public class Visit
    {
        public int Id { get; set; }
        
        [Required]
        public int UserId { get; set; }
        
        [DisplayName("Мастер")]
        public User User { get; set; }

        [Required]
        [DisplayName("Дата")]
        public DateTime Date { get; set; }

        [DisplayName("Время")]
        public DateTime TimeBegin { get; set; }
        
        [Required]
        public int PetId { get; set; }

        [DisplayName("Питомец")]
        public Pet Pet { get; set; }

        [DisplayName("Уровень агрессии")]
        public int AggressionLevel { get; set; }
        
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

        [DisplayName("Сумма")]
        public int Sum { get; set; }

        [Required]
        public int StatusId { get; set; }
        
        [DisplayName("Статус")]
        public VisitStatus Status { get; set; }

        [DisplayName("Оплата")]
        public ICollection<Payment> Payments { get; set; }

        [DisplayName("Процедуры")]
        public ICollection<VisitProcedure> VisitProcedures { get; set; }

        public Visit()
        {
            Payments = new List<Payment>();
            VisitProcedures = new List<VisitProcedure>();
        }
    }
}