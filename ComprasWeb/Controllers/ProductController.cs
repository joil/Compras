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
	[Scaffolding(typeof(Product))]
	[Layout("default")]
	[Rescue("generalerror")]
	[FilterAttribute(ExecuteWhen.BeforeAction, typeof(SecurityFilter),ExecutionOrder=0)]
	[FilterAttribute(ExecuteWhen.BeforeAction, typeof(AdminFilter),ExecutionOrder=1)]
	public class ProductController : SmartDispatcherController
	{
		public void @New() {
			PropertyBag["familyproducts"] = FamilyProduct.FindAll ();
		}
	}
}

