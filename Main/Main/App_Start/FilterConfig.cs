using Main.Services;
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
			private readonly ILogger _logger = new LogService();
			public override void OnException(ExceptionContext filterContext)
			{
				_logger.Error("Error", filterContext.Exception);
			}
		}
	}
}
