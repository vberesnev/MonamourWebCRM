using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace MonamourWeb.ViewModels
{
    public abstract class BaseViewModel<T>
        where T: class, new()
    {
        public PageSettings PageSettings { get; set; }
        public PaginatedList<T> PaginatedList { get; set; }
        public int Count => PaginatedList.Count;
    }



    public class PageSettings
    {
        public string Sort { get; set; }
        public string Search { get; set; }
        public int? PageSize { get; set; }
        public IEnumerable<SelectListItem> PageSizes { get; set; }
    }
}