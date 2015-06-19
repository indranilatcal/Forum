using Main.Forum.Models;
using Main.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web;

namespace Main.Services
{
	public interface IForumService
	{
		Task<MembershipUser> GetUserAsync(string username);
		Task RegisterAsync(MemberAddModel model);
	}
	public class ForumService : IForumService
	{
		private const string _forumBaseApiKey = "ForumBaseApi";
		private static readonly Uri _baseUri;

		static ForumService()
		{
			_baseUri = new Uri(ConfigurationManager.AppSettings[_forumBaseApiKey]);
		}

		public async Task<MembershipUser> GetUserAsync(string username)
		{
			using (var apiClient = GetClient())
			{
				MembershipUser user = null;
				var response = await apiClient.GetAsync(string.Format("Registration/{0}", username));
				if(response.IsSuccessStatusCode)
					user = await response.Content.ReadAsAsync<MembershipUser>();

				return user;
			}
		}


		public async Task RegisterAsync(MemberAddModel member)
		{
			using (var apiClient = GetClient())
			{
				var response = await apiClient.PostAsJsonAsync("Registration", member);
				response.EnsureSuccessStatusCode();
			}
		}

		private static HttpClient GetClient()
		{
			var client = new HttpClient();

			client.BaseAddress = _baseUri;
			client.DefaultRequestHeaders.Accept.Clear();
			client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

			return client;
		}
	}
}