using System.Web;
using System.Web.Mvc;

namespace Main
{
	public class FilterConfig
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new LogErrorFilter());
		}

		public class LogErrorFilter : HandleErrorAttribute
		{
			public override void OnException(ExceptionContext filterContext)
			{
				base.OnException(filterContext);
			}
		}
	}
}
