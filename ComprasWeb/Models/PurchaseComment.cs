using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class PurchaseComment : ActiveRecordValidationBase<PurchaseComment>
	{
		private Purchase _purchase;

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[Property]
		public string Comment {
			get; set; }

		[Property]
		public string UserComment {
			get; set;
		}

		[Property]
		public DateTime Created {
			get; set;
		}

		[BelongsTo("Purchase")]
		public Purchase Purchase
		{
			get { return _purchase; }
			set { _purchase = value; }
		}
	}
}

