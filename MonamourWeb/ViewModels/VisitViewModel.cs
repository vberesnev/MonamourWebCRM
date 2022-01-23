using System;
using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class VisitViewModel
    {
        public Visit Visit { get; set; }
        public IEnumerable<User> Masters { get; set; }
        public IEnumerable<Animal> Animals { get; set; }
        public IEnumerable<PaymentType> PaymentTypes { get; set; }
        public ProceduresForVisitCard ProceduresForVisitCard { get; set; }

        public VisitViewModel()
        {
            Visit = new Visit();
            ProceduresForVisitCard = new ProceduresForVisitCard();
        }
    }
}