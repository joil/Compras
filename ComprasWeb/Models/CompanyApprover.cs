using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;
using System.Security.Cryptography;
using System.Security.Principal;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class CompanyApprover : ActiveRecordValidationBase<CompanyApprover>
	{
		private Company _company;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[BelongsTo("Company")]
		public Company Company
		{
			get { return _company; }
			set { _company = value; }
		}

		[Property]
		public string Name {
			get; set; }

		[Property]
		public string Login {
			get; set; }

		[Property]
		public string Domain {
			get; set; }

		public static CompanyApprover FindById(int id) {
			return (CompanyApprover) FindByPrimaryKey (id);
		}

		public static CompanyApprover[] FindByLogin(string login) {
			return (CompanyApprover[]) FindAllByProperty("Login",login);
		}
	}
}

