using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Main.Forum.Models
{
	public class MemberAddModel
	{
		public string UserName { get; set; }

		public string Email { get; set; }

		public string Password { get; set; }

		public bool IsApproved { get; set; }

		public string Comment { get; set; }

		public string SpamAnswer { get; set; }

		public string SocialProfileImageUrl { get; set; }

		public LoginType LoginType { get; set; }

		public string[] Roles { get; set; }
	}

	public enum LoginType
	{
		Facebook,
		Google,
		Standard
	}
}