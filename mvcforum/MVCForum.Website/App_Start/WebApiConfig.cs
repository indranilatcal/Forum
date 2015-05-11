using Microsoft.Practices.Unity;
using MVCForum.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;

namespace MVCForum.Website
{
	public class WebApiConfig
	{
		public static void Register(HttpConfiguration config, IUnityContainer container)
		{
			// Web API configuration and services
			config.DependencyResolver = new UnityWebApiDependencyResolver(container);
			// Web API routes
			config.MapHttpAttributeRoutes();

			config.Routes.MapHttpRoute(
				name: "DefaultApi",
				routeTemplate: "api/{controller}/{id}",
				defaults: new { id = RouteParameter.Optional }
			);
		}
	}
}