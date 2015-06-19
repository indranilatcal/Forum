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
		Task<MembershipUser> GetUserAsync(string universalId);
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

		public async Task<MembershipUser> GetUserAsync(string universalId)
		{
			using (var apiClient = GetClient())
			{
				MembershipUser user = null;
				var response = await apiClient.GetAsync(string.Format("Registration/{0}", universalId));
				if(response.IsSuccessStatusCode)
					user = await response.Content.ReadAsAsync<MembershipUser>();
				else if(response.StatusCode != System.Net.HttpStatusCode.NotFound)
				{
					/*
					 * Treat not found as special case as forum user may not be found due to other errors, e.g. site being down
					 * Need to handle such cases as genuine errors on the main site & not as member not registered/found
					 */
					throw new Exception(string.Format("Forum Error: {0}", response.ReasonPhrase));
				}

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