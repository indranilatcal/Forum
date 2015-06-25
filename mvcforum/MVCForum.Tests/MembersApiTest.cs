using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using MVCForum.Website.ViewModels;

namespace MVCForum.Tests
{
	[TestClass]
	public class MembersApiTest
	{
		[TestMethod]
		public void TopicsPostedInForNonExistentMemberShouldReturnNotFoundError()
		{
			using (var client = ApiHelper.GetClient())
			{
				var response = client.GetAsync(BuildUrlFragment("junk")).Result;
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
			}
		}

		[TestMethod]
		public void ShouldNotThrowErrorWhenNoTopicsPostedInFoundForUser()
		{
			using (var client = ApiHelper.GetClient())
			{
				var userWithoutPosts = "f95950b5-8f89-4319-867a-a829c5633a31";
				var response = client.GetAsync(BuildUrlFragment(userWithoutPosts)).Result;
				response.EnsureSuccessStatusCode();
				var postedIn = response.Content.ReadAsAsync<PostedInViewModel>().Result;
				Assert.AreEqual(0, postedIn.Topics.Count);
			}
		}

		[TestMethod]
		public void ShouldReturnTopicsPostedInForAUser()
		{
			using (var client = ApiHelper.GetClient())
			{
				var userWithPosts = "d686b149-d11f-4827-9489-1b6abf63a802";
				var response = client.GetAsync(BuildUrlFragment(userWithPosts)).Result;
				response.EnsureSuccessStatusCode();
				var postedIn = response.Content.ReadAsAsync<PostedInViewModel>().Result;
				Assert.AreEqual(1, postedIn.Topics.Count);
			}
		}

		private string BuildUrlFragment(string id)
		{
			return string.Format("Members/{0}/TopicsPostedIn", id);
		}
	}
}
