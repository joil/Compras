using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

using Castle.MonoRail.Framework;

using ComprasWeb.Controllers;
using ComprasWeb.Models;

namespace ComprasWeb.Filters
{
	public class SecurityFilter : IFilter
	{
		public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext) {
			context.Session["jumpTo"] = context.Request.Url;

			if (context.Session.Contains("user")) {
				User user = (User)context.Session["user"];
				context.CurrentUser = user;
				return true;
			} else {
				string cookieValue = context.Request.ReadCookie("user");
				if (cookieValue != null)
				{
					User user = Utilities.GetUserByCookie(cookieValue);
					context.Session["user"] = user;
					context.CurrentUser = user;

					return true;
				} else {
					context.Response.RedirectToUrl ("/login/index");
					return false;
				}
			}
		}
	}
}

