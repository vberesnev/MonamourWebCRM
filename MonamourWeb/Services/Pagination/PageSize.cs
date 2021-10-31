using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MonamourWeb.Services.Pagination
{
    public static class PageSize
    {
        public static IEnumerable<SelectListItem> GetSelectListItems(int? selectedValue)
        {
            var list = new List<SelectListItem>()
            {
                new SelectListItem("1", "1", false),
                new SelectListItem("3", "3", false),
                new SelectListItem("5", "5", false),
                new SelectListItem("10", "10", false),
                new SelectListItem("25", "25", true),
                new SelectListItem("50", "50", false),
                new SelectListItem("100", "100", false)
            };

            if (selectedValue is null or 25)
                return list;

            foreach (var item in list)
            {
                if (item.Value == selectedValue.ToString())
                {
                    item.Selected = true;
                    break;
                }
            }

            return list;
        }
    }
}