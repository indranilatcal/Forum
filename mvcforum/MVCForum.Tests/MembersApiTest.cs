using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

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
				var response = client.GetAsync("Members/junk/TopicsPostedIn").Result;
				Assert.AreEqual(HttpStatusCode.NotFound, response.StatusCode);
			}
		}

		[TestMethod]
		public void ShouldNotThrowErrorWhenNoPostedTopicsFoundForUser()
		{

		}
	}
}
