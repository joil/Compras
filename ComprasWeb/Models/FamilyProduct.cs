using System;
using Castle.ActiveRecord;
using System.Collections;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class FamilyProduct : ActiveRecordValidationBase<FamilyProduct>
	{
		private ISet<Product> _products;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[Property]
		public string Name {
			get; set; }

		[HasMany(typeof(Product), Inverse = true, Cascade = ManyRelationCascadeEnum.All)]
		public ISet<Product> Products
		{
			get { return _products; }
			set { _products = value; }
		}

		public static FamilyProduct FindById(int id) {
			return (FamilyProduct) FindByPrimaryKey (id);
		}
	}
}

