using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class Company : ActiveRecordValidationBase<Company>
	{
		private ISet<CompanyApprover> _companyapprover;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[Property]
		public string Name {
			get; set; }

		[HasMany(typeof(CompanyApprover), Inverse = true, Cascade = ManyRelationCascadeEnum.All)]
		public ISet<CompanyApprover> CompanyApprovers
		{
			get { return _companyapprover; }
			set { _companyapprover = value; }
		}

		public static Company FindById(int id) {
			return (Company) FindByPrimaryKey (id);
		}
	}
}
