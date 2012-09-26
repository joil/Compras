using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class PurchaseStatusChange : ActiveRecordValidationBase<PurchaseStatusChange>
	{
		private Purchase.StatusPurchase _statusold;
		private Purchase.StatusPurchase _statusnew;
		private Purchase _purchase;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[Property]
		public DateTime Created {
			get; set; }

		[Property]
		public Purchase.StatusPurchase StatusOld {
			get { return _statusold; }
			set { _statusold = value; }
		}

		[Property]
		public Purchase.StatusPurchase StatusNew {
			get { return _statusnew; }
			set { _statusnew = value; }
		}

		[Property]
		public string author {
			get; set; }

		[BelongsTo("Purchase")]
		public Purchase Purchase
		{
			get { return _purchase; }
			set { _purchase = value; }
		}
	}
}

