using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class Budget : ActiveRecordValidationBase<Budget>
	{
		private Company _company;
		private FamilyProduct _familyproduct;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[Property]
		public float Value {
			get; set; }

		[Property]
		public DateTime Created {
			get; set; }

		[BelongsTo("Company")]
		public Company Company
		{
			get { return _company; }
			set { _company = value; }
		}

		[BelongsTo("Fam")]
		public FamilyProduct FamilyProduct
		{
			get { return _familyproduct; }
			set { _familyproduct = value; }
		}

		public static Budget FindByCompanyFamilyProduct(Company company, FamilyProduct familyproduct) {
			return (Budget) FindFirst(typeof(Budget),new Order[] { Order.Desc("Created") },new ICriterion[] { Expression.Eq("Company", company), Expression.Eq("FamilyProduct",familyproduct)});

		}
	}
}

