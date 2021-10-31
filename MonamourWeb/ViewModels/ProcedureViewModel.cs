using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class ProcedureViewModel
    {
        public Procedure Procedure { get; set; }
        public IEnumerable<Animal> Animals { get; set; }
    }
}