using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace MVCForum.Tests
{
	internal static class ApiHelper
	{
		private const string _forumBaseApiKey = "ForumBaseApi";
		private static readonly Uri _baseUri;
		static ApiHelper()
		{
			_baseUri = new Uri(ConfigurationManager.AppSettings[_forumBaseApiKey]);
		}

		internal static HttpClient GetClient()
		{
			var client = new HttpClient();

			client.BaseAddress = _baseUri;
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}
	}
}
