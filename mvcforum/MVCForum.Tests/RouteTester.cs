
using System;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Hosting;
using System.Web.Http.Routing;
namespace MVCForum.Tests
{
	internal class RouteTester
	{
		private readonly HttpConfiguration _config;
		private readonly HttpRequestMessage _request;
		private readonly IHttpRouteData _routeData;
		private readonly IHttpControllerSelector _controllerSelector;
		private readonly HttpControllerContext _controllerContext;

		internal RouteTester(HttpConfiguration config, HttpRequestMessage request)
		{
			_config = config;
			_request = request;
			_routeData = config.Routes.GetRouteData(request);
			_request.Properties[HttpPropertyKeys.HttpRouteDataKey] = _routeData;
			_controllerSelector = new DefaultHttpControllerSelector(_config);
			_controllerContext = new HttpControllerContext(_config, _routeData, _request);
		}

		internal Type GetControllerType()
		{
			var descriptor = _controllerSelector.SelectController(_request);
			_controllerContext.ControllerDescriptor = descriptor;
			return descriptor.ControllerType;
		}

		internal string ActionName
		{
			get
			{
				if (_controllerContext.ControllerDescriptor == null)
					GetControllerType();

				var apiActionSelector = new ApiControllerActionSelector();
				var descriptor = apiActionSelector.SelectAction(_controllerContext);

				return descriptor.ActionName;
			}
		}
	}
}
