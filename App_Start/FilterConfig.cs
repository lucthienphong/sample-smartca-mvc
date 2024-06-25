using System.Web;
using System.Web.Mvc;

namespace Sample.WebApp.SmartCAv4
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
