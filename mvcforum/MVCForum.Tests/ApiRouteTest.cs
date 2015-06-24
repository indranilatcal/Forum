using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Http;
using System.Net.Http;
using MVCForum.Website.Controllers.ApiControllers;
using MVCForum.Website;

namespace MVCForum.Tests
{
	[TestClass]
	public class ApiRouteTest
	{
		private readonly HttpConfiguration _config;

		public ApiRouteTest()
		{
			_config = new HttpConfiguration();
			WebApiConfig.RegisterRoutes(_config);
			_config.EnsureInitialized();
		}

		[TestMethod]
		public void TopicGetByIdRouteIsCorrect()
		{
			var req = new HttpRequestMessage(HttpMethod.Get, "http://localhost/api/Topic/blah");
			var routeTester = new RouteTester(_config, req);
			Assert.AreEqual(typeof(TopicController), routeTester.GetControllerType());
			Assert.AreEqual(ReflectionHelper.GetMethodName((TopicController t) => t.Get(string.Empty)), routeTester.ActionName);
		}
	}
}
