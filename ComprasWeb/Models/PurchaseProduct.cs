using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class PurchaseProduct : ActiveRecordValidationBase<PurchaseProduct>
	{
		private Purchase _purchase;
		private Product _product;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[BelongsTo("Purchase")]
		public Purchase Purchase
		{
			get { return _purchase; }
			set { _purchase = value; }
		}

		[BelongsTo("Product")]
		public Product Product
		{
			get { return _product; }
			set { _product = value; }
		}

		[Property]
		public string Comment {
			get; set; }

		[Property]
		public float Quantity {
			get; set; }

		[Property]
		public string EstimatedPrice {
			get; set; }

		public bool Checked {
			get; set; }
	}
}

