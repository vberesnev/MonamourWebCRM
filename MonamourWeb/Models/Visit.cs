using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace MonamourWeb.Models
{
    public class Visit : ICloneable
    {
        public int Id { get; set; }
        
        [Required(ErrorMessage = "Посещению должен быть назначен мастер")]
        public int UserId { get; set; }
        
        [DisplayName("Мастер")]
        public User User { get; set; }

        [Required(ErrorMessage = "Посещению должна быть назначена дата")]
        [DisplayName("Дата")]
        public DateTime Date { get; set; }

        [DisplayName("Время")]
        public DateTime TimeBegin { get; set; }
        
        [Required(ErrorMessage = "Посещению должен быть назначен питомец")]
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

        [Required(ErrorMessage = "Посещению должна быть назначен статус")]
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

        public override string ToString()
        {
            var payments = string.Join(" ", Payments.Select(x => x.Sum + " (" + x.PaymentTypeId + ") [" + x.Id + "]"));
            var procedures = string.Join(" ", VisitProcedures.Select(x => x.Procedure.Title + " [" + x.Id + "]"));

            return $"Посещение: [Id]: {Id}; [Id мастера]: {UserId}; [Мастер]: {User.Name}; [Дата]: {Date}; [Время]: {TimeBegin}; " +
                   $"[Id питомца]: {PetId}; [Питомец]: {Pet?.Name} [{Pet?.Id}]; [Инфо]: {Info}; [Уровень агрессии]: {AggressionLevel}; " +
                   $"[Сумма]: {Sum}; [Статус]: {Status?.Status}; [Процедуры]: {procedures}; [Оплата]: {payments};";
        }

        public object Clone()
        {
            return new Visit()
            {
                Id = this.Id,
                UserId = this.UserId,
                User = this.User?.Clone() as User,
                Date = this.Date,
                TimeBegin = this.TimeBegin,
                PetId = this.PetId,
                Pet = this.Pet?.Clone() as Pet,
                Info = this.Info,
                AggressionLevel = this.AggressionLevel,
                Sum = this.Sum,
                Status = this.Status?.Clone() as VisitStatus,
                VisitProcedures = this.VisitProcedures,
                Payments = this.Payments
            };
        }
    }
}