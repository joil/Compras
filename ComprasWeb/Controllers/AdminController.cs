using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Castle.ActiveRecord;
using Castle.MonoRail.Framework;
using Castle.MonoRail.ActiveRecordSupport;
using Castle.MonoRail.Framework.Helpers;
using NHibernate.Criterion;

using ComprasWeb.Models;
using ComprasWeb.Filters;

namespace ComprasWeb.Controllers
{
	[Layout("default")]
	[Rescue("generalerror")]
	[FilterAttribute(ExecuteWhen.BeforeAction, typeof(SecurityFilter),ExecutionOrder=0)]
	[FilterAttribute(ExecuteWhen.BeforeAction, typeof(AdminFilter),ExecutionOrder=1)]
	public class AdminController : SmartDispatcherController
	{
		public void AllPurchase(int company, Purchase.StatusPurchase statusid,string findid, int page) {
			int id;
			PropertyBag["companys"] = Company.FindAll (new Order[] { Order.Asc("Name") });
			PropertyBag["status"] = Enum.GetValues(typeof(Purchase.StatusPurchase));

			if (findid != null) {
				if (int.TryParse (findid, out id)) {
					//RedirectToUrl ("/purchase/view?idpurchase="+id.ToString ());
					RedirectToAction ("editpurchase","id="+id.ToString ());
				}
			}
			else {
				if (company == 0) {
					PropertyBag["purchases"] = PaginationHelper.CreatePagination((IList)Purchase.FindAll(new Order[] { Order.Desc("DateRequest")}, new ICriterion[] { Expression.Eq("Status",statusid)}), 15, page);
				}
				else {
					Company cpny = Company.FindById (company);
					PropertyBag["purchases"] = PaginationHelper.CreatePagination((IList)Purchase.FindAll(new Order[] { Order.Desc("DateRequest")}, new ICriterion[] { Expression.Eq("Status",statusid), Expression.Eq("Company",cpny)}), 15, page);
				}
			}
			//else
			//	PropertyBag["purchases"] = PaginationHelper.CreatePagination((IList)Purchase.FindAll(new Order[] { Order.Desc("DateRequest")}), 15, page);

			//PropertyBag["tmp"] = statusall.ToString ();
		}

		public void EditPurchase(int id) {
			Purchase purchase = Purchase.FindOne (new ICriterion[] { Expression.Eq("Id",id)});

			if (purchase == null) {
				Flash["TypeMsg"] = "alert alert-Error";
				Flash["Msg"] = "<b>Solicitud de compra No Existe !!!</b> ";
				RedirectToReferrer ();
			}

			PropertyBag["purchase"] = purchase;

			PropertyBag["status"] = Enum.GetValues(typeof(Purchase.StatusPurchaseRestrict));

			PropertyBag["admins"] = User.admins;

			PropertyBag["getdate"] = DateTime.Now.ToShortDateString ();
		}

		public void UpdatePurchase([ARDataBind("purchase",AutoLoadBehavior.Always)] Purchase purchase, string comment) {
			Purchase purchaseold = Purchase.FindById (purchase.Id);

			PurchaseStatusChange psc = new PurchaseStatusChange();
			psc.Created = DateTime.Now;
			psc.Purchase = purchase;
			psc.StatusOld =  purchaseold.Status;
			psc.StatusNew = purchase.Status;
			psc.author = Context.CurrentUser.Identity.Name.ToLower ();

			if ((string.IsNullOrEmpty (purchase.Assigned)) || purchase.Assigned == "0")
				purchase.Assigned = null;

			using (TransactionScope t1 = new TransactionScope())
			{
				try {
					purchase.Save ();
					psc.Save ();

					if (!string.IsNullOrEmpty(comment)) {
						PurchaseComment pc = new PurchaseComment();
						pc.Comment = comment;
						pc.Created = DateTime.Now;
						pc.Purchase = purchase;
						pc.UserComment = Context.CurrentUser.Identity.Name.ToLower ();

						pc.Save ();
					}

					if (!string.IsNullOrEmpty(purchase.UserDelivery) && (!string.IsNullOrEmpty(purchase.DatelikeDelivery.ToString ()))) {
						purchase.MailRequest = Utilities.GetMailByLogin (purchase.UserDelivery.ToLower ());
					}
					t1.VoteCommit();
					Flash["TypeMsg"] = "alert alert-success";
					Flash["Msg"] = "<b>Ok, Actualizado el Estado de la Solicitud</b> ";
				} catch (Exception ex) {
					t1.VoteRollBack();
					Flash["TypeMsg"] = "alert alert-error";
					Flash["Msg"] = "Error :" + ex.Message;
					RedirectToReferrer ();
					return;
				}
			}

			if ((purchase.Status == Purchase.StatusPurchase.Comprado) && (purchase.DatelikeDelivery != null)) {
				DateTime fecha = (DateTime) purchase.DatelikeDelivery;
				PropertyBag["to"] = purchase.MailRequest;
				PropertyBag["subject"] = "[compras] Solicitud de Recepcion ID:" + purchase.Id.ToString ();
				PropertyBag["idpurchase"] = purchase.Id;
				PropertyBag["purchase"] = purchase;
				PropertyBag["purchaseproducts"] = purchase.PurchaseProducts;
				PropertyBag["fecha"] = fecha.ToShortDateString();
				DeliverEmail(RenderMailMessage ("torender",null,PropertyBag));
			}
			RedirectToReferrer ();
		}
		
	}
}

