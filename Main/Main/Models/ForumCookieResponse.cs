﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Main.Models
{
	public class LightResponse
	{
		public bool Success { get; set; }
		public string Reason { get; set; }
		public string Message { get; set; }
	}
	public class ForumCookieResponse : LightResponse
	{
		public string CookieValue { get; set; }
	}
}