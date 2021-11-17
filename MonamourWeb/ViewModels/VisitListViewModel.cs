using System;
using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class VisitListViewModel : BaseViewModel<Visit>
    {
        public DateTime Begin { get; set; }
        public string BeginString => Begin.ToString("d");
        public DateTime End { get; set; }
        public string EndString => End.ToString("d");

        public IEnumerable<VisitStatus> Statuses { get; set; }
        public int? StatusId { get; set; }
        
        public IEnumerable<User> Users { get; set; }
        public int? UserId { get; set; }

        public int TotalSum { get; set; }

        public VisitListViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}