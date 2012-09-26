using System;
using Castle.ActiveRecord;
using Castle.Components;
using Castle.Components.Validator;
using Iesi.Collections.Generic;
using NHibernate.Criterion;

namespace ComprasWeb.Models
{
	[ActiveRecord]
	public class Purchase : ActiveRecordValidationBase<Purchase>
	{
		private StatusPurchase _status;
		private Company _company;
		private FamilyProduct _familyproduct;
		private CompanyApprover _companyapprover;
		private ISet<PurchaseProduct> _purchaseproducs;
		private ISet<PurchaseComment> _purchasecomments;
		private ISet<PurchaseAttachment> _purchaseattachments;

		//public Purchase (UserArea userrequest) {
		//	UserRequest = userrequest;
		//	DatePurchase = System.DateTime.Now;
		//}

		[PrimaryKey(PrimaryKeyType.Native)]
		public int Id {
			get; set; }

		[Property]
		public string UserRequest {
			get; set;
		}

		[Property]
		public DateTime? DateRequest {
			get; set;
		}

		[ValidateNonEmpty("Debe seleccionar el Aprobador")]
		[BelongsTo("CompanyApprover")]
		public CompanyApprover UserApproval {
			get { return _companyapprover; }
			set { _companyapprover = value; }
		}

		[Property]
		public DateTime? DateApproval {
			get; set; }

		[Property]
		public DateTime? DatePurchase {
				get; set;
		}

		[ValidateNonEmpty("Debe seleccionar la Empresa")]
		[BelongsTo("Company")]
		public Company Company
		{
			get { return _company; }
			set { _company = value; }
		}

		[Property]
		public StatusPurchase Status {
			get { return _status; }
			set { _status = value; }
		}

		[BelongsTo("FamilyProduct")]
		public FamilyProduct familyproduct
		{
			get { return _familyproduct; }
			set { _familyproduct = value; }
		}

		[HasMany(typeof(PurchaseProduct), Inverse = true, Cascade = ManyRelationCascadeEnum.All)]
		public ISet<PurchaseProduct> PurchaseProducts
		{
			get { return _purchaseproducs; }
			set { _purchaseproducs = value; }
		}

		[HasMany(typeof(PurchaseComment), Inverse = true, Cascade = ManyRelationCascadeEnum.All)]
		public ISet<PurchaseComment> PurchaseComments
		{
			get { return _purchasecomments; }
			set { _purchasecomments = value; }
		}

		[HasMany(typeof(PurchaseAttachment), Inverse = true, Cascade = ManyRelationCascadeEnum.All)]
		public ISet<PurchaseAttachment> PurchaseAttachments
		{
			get { return _purchaseattachments; }
			set { _purchaseattachments = value; }
		}

		[Property]
		public string Assigned {
			get; set;
		}

		[Property]
		public float? BudgetValue {
			get; set; }

		[Property]
		public string MailRequest {
			get; set;
		}

		[Property]
		public string NameUserRequest {
			get; set;
		}

		[ValidateDateTime("Fecha Posible Entrega, mal ingresada")]
		[Property]
		public DateTime? DatelikeDelivery {
			get; set;
		}

		[Property]
		public DateTime? DateDelivery {
			get; set;
		}

		[Property]
		public string UserDelivery {
			get; set;
		}

		public int QuantityComment {
			get { return PurchaseComments.Count; }
		}

		public static Purchase FindById(int id) {
			return (Purchase) FindByPrimaryKey(id);
		}

		public static Purchase[] FindByMyOrder(string user) {
			return (Purchase[]) FindAll (typeof(Purchase),new Order[] { Order.Desc("DateRequest") }, new ICriterion[] { Expression.Eq("UserRequest",user)});
		}

		public static Purchase[] FindByMyApprover(CompanyApprover[] users) {
			return (Purchase[]) FindAll (typeof(Purchase),new Order[] { Order.Desc("DateRequest") }, new ICriterion[] { Expression.In("UserApproval",users)});
		}

		public enum StatusPurchase {
			En_Aprobacion,
			Aprobada,
			Rechazada,
			Cotizando,
			Comprado,
			Entregado,
			Cancelada
		}

		public enum StatusPurchaseRestrict {
			Aprobada = 1,
			Cotizando = 3,
			Comprado = 4,
			Entregado = 5,
			Cancelada = 6
		}
	}
}

