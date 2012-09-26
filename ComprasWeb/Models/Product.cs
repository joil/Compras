using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class Product : ActiveRecordValidationBase<Product>
	{
		private FamilyProduct _familyproduct;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[Property]
		public string Name {
			get; set; }

		[BelongsTo("FamilyProduct")]
		public FamilyProduct FamilyProduct
		{
			get { return _familyproduct; }
			set { _familyproduct = value; }
		}

		public static Product FindById(int id) {
			return (Product) FindByPrimaryKey(id);
		}

		public static Product[] FindByFamily(FamilyProduct familyproduct) {
			return (Product[]) FindAll (typeof(Product),new Order[] { Order.Asc("Name") }, new ICriterion[] { Expression.Eq("FamilyProduct",familyproduct)});
		}
	}
}

