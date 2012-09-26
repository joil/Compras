using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

using Castle.MonoRail.Framework;

using ComprasWeb.Controllers;
using ComprasWeb.Models;

namespace ComprasWeb.Filters
{
	public class AdminFilter : IFilter
	{
		public bool Perform(ExecuteWhen exec, IEngineContext context, IController controller, IControllerContext controllerContext) {
			/*
			ArrayList admins = new ArrayList();
			admins.Add("jpino");
			admins.Add("logas");
			admins.Add("lmolina");

			if (admins.Contains (context.CurrentUser.Identity.Name.ToLower()))
				return true;
			else {
				context.Flash["TypeMsg"] = "alert alert-error";
				context.Flash["Msg"] = "Error : No eres admin";

				context.Response.RedirectToUrl ("/");
				return false;
			}
			*/
			if (context.CurrentUser.IsInRole ("admin"))
				return true;
			else {
				context.Flash["TypeMsg"] = "alert alert-error";
				context.Flash["Msg"] = "Error : No eres admin";

				context.Response.RedirectToUrl ("/");
				return false;
			}
		}
	}
}

