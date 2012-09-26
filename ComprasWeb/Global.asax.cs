
using System;
using System.Collections;
using System.ComponentModel;
using System.Web;
using System.Web.SessionState;

using Castle.ActiveRecord;
using Castle.ActiveRecord.Framework.Config;
using Castle.MonoRail.Framework.Routing;

using ComprasWeb.Models;

namespace ComprasWeb
{
	public class Global : System.Web.HttpApplication
	{
		
		protected virtual void Application_Start (Object sender, EventArgs e)
		{
			Castle.ActiveRecord.Framework.IConfigurationSource config = ActiveRecordSectionHandler.Instance;
			ActiveRecordStarter.Initialize(config,
			                               typeof(Budget),
			                               typeof(Product), 
			                               typeof(FamilyProduct),
			                               typeof(Purchase),
			                               typeof(PurchaseAttachment),
			                               typeof(PurchaseComment),
			                               typeof(PurchaseProduct),
			                               typeof(PurchaseStatusChange),
			                               typeof(Company),
			                               typeof(CompanyApprover));
			//ActiveRecordStarter.UpdateSchema();
			log4net.Config.XmlConfigurator.Configure();
			RegisterRoutes(RoutingModuleEx.Engine);
		}

		private static void RegisterRoutes(IRoutingRuleContainer rules)
		{
			rules.Add(new PatternRoute("")
					.DefaultForController().Is("login")
					.DefaultForAction().Is("index")
			);
			
			rules.Add(new PatternRoute("/[controller]/[action]")
					.DefaultForAction().Is("index"));
		}
		
		protected virtual void Session_Start (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_BeginRequest (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_EndRequest (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_AuthenticateRequest (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_Error (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Session_End (Object sender, EventArgs e)
		{
		}
		
		protected virtual void Application_End (Object sender, EventArgs e)
		{
		}
	}
}

