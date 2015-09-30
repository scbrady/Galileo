using Galileo.Filters;
using System.Web;
using System.Web.Mvc;

namespace Galileo
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new AuthorizeUser());
            filters.Add(new HandleErrorAttribute());
        }
    }
}
