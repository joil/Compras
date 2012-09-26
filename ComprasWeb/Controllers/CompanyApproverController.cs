using System;
using System.Collections.Generic;
using System.IO;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using NHibernate.Criterion;

using ComprasWeb.Models;
using ComprasWeb.Filters;

namespace ComprasWeb.Controllers
{
	[Scaffolding(typeof(CompanyApprover))]
	[Layout("default")]
	[Rescue("generalerror")]
	[FilterAttribute(ExecuteWhen.BeforeAction, typeof(SecurityFilter),ExecutionOrder=0)]
	[FilterAttribute(ExecuteWhen.BeforeAction, typeof(AdminFilter),ExecutionOrder=1)]
	public class CompanyApproverController : SmartDispatcherController
	{

		public void List() {
			PropertyBag["companyapprovers"] = CompanyApprover.FindAll (new Order[] { Order.Asc("Company") });
			//PropertyBag["companys"] = Company.FindAll (new Order[] { Order.Asc("Name") });
		}

		public void @New() {
			PropertyBag["companys"] = Company.FindAll ();
		}

		public void Edit(int id) {
			PropertyBag["companyapprover"] = CompanyApprover.FindById (id);
			PropertyBag["companys"] = Company.FindAll ();
		}
	}
}

