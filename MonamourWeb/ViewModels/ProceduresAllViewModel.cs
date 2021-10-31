using System.Collections.Generic;
using MonamourWeb.Models;

namespace MonamourWeb.ViewModels
{
    public class ProceduresAllViewModel : BaseViewModel<Procedure>
    {
        public IEnumerable<Animal> Animals { get; set; }
        public int? AnimalId { get; set; }

        public ProceduresAllViewModel()
        {
            PageSettings = new PageSettings();
        }
    }
}