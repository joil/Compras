using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Web;
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
	[FilterAttribute(ExecuteWhen.BeforeAction, typeof(SecurityFilter))]
	public class PurchaseController : ARSmartDispatcherController
	{
		public void @new(int familyproduct, int company, string comment) {

			PropertyBag["familyproducts"] = FamilyProduct.FindAll (new Order[] { Order.Asc("Name") });
			if (familyproduct > 0)
				//PropertyBag["products"] = FamilyProduct.FindById (familyproduct).Products;
				PropertyBag["products"] = Product.FindByFamily (FamilyProduct.FindById (familyproduct));

			PropertyBag["companys"] = Company.FindAll (new Order[] { Order.Asc("Name") });
			if (company > 0)
				PropertyBag["approvers"] = Company.FindById(company).CompanyApprovers;

			PropertyBag["comment"] = comment;
		}

		public void submit ([ARDataBind("purchaseproducts")] PurchaseProduct[] purchaseproducts, int companyid, int userapproval, int familyproductid, string comment, HttpPostedFile uploadedFile) {

			Purchase purchase = new Purchase();
			purchase.UserRequest = Context.CurrentUser.Identity.Name.ToLower ();
			purchase.DateRequest = System.DateTime.Now;

			User user = (User) Context.Session["user"];
			purchase.MailRequest = user.Mail;
			purchase.NameUserRequest = user.Cn;


			if (companyid > 0)
				purchase.Company = Company.FindById(companyid);
			else {
				Flash["TypeMsg"] = "alert alert-error";
				Flash["Msg"] = "Seleccionar Compañia";
				RedirectToAction("new");
				return;
			}

			CompanyApprover ca = new CompanyApprover();
			if (userapproval > 0) {
				ca = CompanyApprover.FindById(userapproval);
				purchase.UserApproval = ca;
			} else {
				Flash["TypeMsg"] = "alert alert-error";
				Flash["Msg"] = "Seleccionar Aprobador";
				RedirectToAction("new");
				return;
			}

			if (familyproductid < 1) {
				Flash["TypeMsg"] = "alert alert-error";
				Flash["Msg"] = "Seleccionar Familia de Productos";
				RedirectToAction("new");
				return;
			}

			purchase.Status = Purchase.StatusPurchase.En_Aprobacion;

			using (TransactionScope t1 = new TransactionScope())
			{
				try {
					//purchase.BudgetValue = Budget.FindByCompanyFamilyProduct(Company.FindById(companyid), FamilyProduct.FindById(familyproductid)).Value;
					purchase.familyproduct = FamilyProduct.FindById(familyproductid);
					purchase.Save ();
					int cont = 0;
					foreach(PurchaseProduct pp in purchaseproducts)
					{
						if (pp.Checked) {
							pp.Purchase = purchase;
							//pp.Product = Product.FindById(1);


							if (!pp.IsValid()) {
								Flash["TypeMsg"] = "alert alert-error";
								Flash["Msg"] = Utilities.GetValidationErrorText(pp.ValidationErrorMessages);
								t1.VoteRollBack ();
								RedirectToAction("new");
								return;
							}
							pp.Save();
							cont++;
						}
					}

					if (cont == 0) {
						t1.VoteRollBack ();
						Flash["TypeMsg"] = "alert alert-error";
						Flash["Msg"] = "Seleccionar Productos";
						RedirectToAction("new");
						return;
					}

					if (!string.IsNullOrEmpty(comment)) {
						PurchaseComment pc = new PurchaseComment();
						pc.Comment = comment;
						pc.Created = DateTime.Now;
						pc.Purchase = purchase;
						pc.UserComment = Context.CurrentUser.Identity.Name.ToLower ();

						pc.Save ();
					}

					if (uploadedFile != null) {
						PurchaseAttachment pa = new PurchaseAttachment();
						pa.Purchase = purchase;
						pa.Name = uploadedFile.FileName;
						pa.Created = DateTime.Now;
						pa.Path = Guid.NewGuid().ToString("N") + uploadedFile.FileName;
						pa.Save ();
						uploadedFile.SaveAs("../upload_compras/"+pa.Path);
					}
					t1.VoteCommit ();
					Flash["TypeMsg"] = "alert alert-success";
					Flash["Msg"] = "Solicitud generada con ID :<b>" + purchase.Id.ToString() + "</b>";
				}
				catch (Exception ex) {
					t1.VoteRollBack ();
					Flash["TypeMsg"] = "alert alert-error";
					if (!purchase.IsValid()) {
						Flash["Msg"] = Utilities.GetValidationErrorText(purchase.ValidationErrorMessages);
					} else {
						Flash["Msg"] = "Error :" + ex.Message;
					}
					RedirectToAction("new");
					return;
				}
			}
			PropertyBag["to"] = ca.Login + "@" + ca.Domain;
			PropertyBag["from"] = ConfigurationManager.AppSettings["mailadmin"];
			PropertyBag["subject"] = "[compras] Solicitud Aprobacion ID:" + purchase.Id.ToString ();
			PropertyBag["idpurchase"] = purchase.Id;
			PropertyBag["username"] = purchase.NameUserRequest;
			PropertyBag["datepurchase"] = purchase.DateRequest.ToString ();
			DeliverEmail(RenderMailMessage ("approver",null,PropertyBag));
		}

		public void Approver(int idpurchase) {

			Purchase purchase = Purchase.FindById(idpurchase);
			if ((purchase.Status == Purchase.StatusPurchase.En_Aprobacion) && (purchase.UserApproval.Login.ToLower () == Context.CurrentUser.Identity.Name.ToLower ())) {
				PropertyBag["purchase"] = purchase;
				PropertyBag["purchaseproducts"] = purchase.PurchaseProducts;

				PropertyBag["budget"] = Budget.FindByCompanyFamilyProduct(purchase.Company, purchase.familyproduct);
			} else {
				Flash["TypeMsg"] = "alert alert-error";
				Flash["Msg"] = "Esta solicitud esta en status :" + purchase.Status.ToString ();

				RedirectToAction("view","idpurchase="+purchase.Id);
			}
		}

		public void Approval () {
		}

		public void Reject () {
		}

		public void ToApprove ([ARDataBind("purchase",AutoLoadBehavior.Always)] Purchase purchase, string response, string comment) {
			PurchaseStatusChange psc = new PurchaseStatusChange();
			psc.Created = DateTime.Now;
			psc.Purchase = purchase;
			psc.StatusOld =  purchase.Status;

			switch (response) {
				case "approval" : {
					purchase.Status = Purchase.StatusPurchase.Aprobada;
					break;
				}
				case "reject" : {
					purchase.Status = Purchase.StatusPurchase.Rechazada;
					break;
				}
				default : {
					Flash["TypeMsg"] = "alert alert-error";
					Flash["Msg"] = "Error :" + response;
					RedirectToReferrer ();
					return;
				}
			}

			psc.StatusNew = purchase.Status;
			psc.author = Context.CurrentUser.Identity.Name.ToLower ();

			purchase.DateApproval = System.DateTime.Now;

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
					t1.VoteCommit();
					Flash["TypeMsg"] = "alert alert-success";
					Flash["Msg"] = "<b>Ok, Solicitud " + purchase.Status +" enviada a Compras</b> ";
				} catch (Exception ex) {
					t1.VoteRollBack();
					Flash["TypeMsg"] = "alert alert-error";
					Flash["Msg"] = "Error :" + ex.Message;
					RedirectToReferrer ();
					return;
				}
			}

			PropertyBag["to"] = ConfigurationManager.AppSettings["mailadmin"];
			PropertyBag["cc"] = purchase.UserRequest + "@" + ConfigurationManager.AppSettings["defaultdomain"];
			PropertyBag["from"] = ConfigurationManager.AppSettings["mailadmin"];
			PropertyBag["subject"] = "[compras] Compra "+ purchase.Status +" ID:" + purchase.Id.ToString ();
			PropertyBag["purchase"] = purchase;
			PropertyBag["purchaseproducts"] = purchase.PurchaseProducts;
			DeliverEmail(RenderMailMessage ("toapprove",null,PropertyBag));

			if (purchase.Status == Purchase.StatusPurchase.Aprobada)
				RenderView ("approval");
			else
				RenderView ("reject");
		}

		public void View(int idpurchase) {
			Purchase purchase = Purchase.FindById(idpurchase);
			PropertyBag["purchase"] = purchase;
			PropertyBag["purchaseproducts"] = purchase.PurchaseProducts;
		}

		public void MyOrder() {
			PropertyBag["purchases"] = Purchase.FindByMyOrder(Context.CurrentUser.Identity.Name.ToLower ());
		}

		public void MyApprover() {
			CompanyApprover[] ca = CompanyApprover.FindByLogin(Context.CurrentUser.Identity.Name.ToLower ());
			PropertyBag["open"] = Purchase.StatusPurchase.En_Aprobacion;
			if ((ca != null) && (ca.Length > 0))
				PropertyBag["purchases"] = Purchase.FindByMyApprover(ca);
		}

		public void Render(int idpurchase) {
			Purchase purchase = Purchase.FindById(idpurchase);
			if ((purchase.Status == Purchase.StatusPurchase.Comprado) && (purchase.UserDelivery.ToLower() == Context.CurrentUser.Identity.Name.ToLower ())) {
				PropertyBag["purchase"] = purchase;
				PropertyBag["purchaseproducts"] = purchase.PurchaseProducts;

				PropertyBag["budget"] = Budget.FindByCompanyFamilyProduct(purchase.Company, purchase.familyproduct);
			} else {
				Flash["TypeMsg"] = "alert alert-error";
				Flash["Msg"] = "No corresponde Recepcionar";

				RedirectToAction("view","idpurchase="+purchase.Id);
			}
		}

		public void ToRender ([ARDataBind("purchase",AutoLoadBehavior.Always)] Purchase purchase, string comment) {

			PurchaseStatusChange psc = new PurchaseStatusChange();
			psc.Created = DateTime.Now;
			psc.Purchase = purchase;
			psc.StatusOld =  purchase.Status;

			psc.StatusNew =Purchase.StatusPurchase.Entregado;
			psc.author = Context.CurrentUser.Identity.Name.ToLower ();

			purchase.DateDelivery = System.DateTime.Now;
			purchase.Status = Purchase.StatusPurchase.Entregado;

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
					t1.VoteCommit();
					Flash["TypeMsg"] = "alert alert-success";
					Flash["Msg"] = "<b>Ok, Solicitud " + purchase.Status +" entregada conforme...</b> ";
				} catch (Exception ex) {
					t1.VoteRollBack();
					if (!purchase.IsValid()) {
						Flash["Msg"] = Utilities.GetValidationErrorText(purchase.ValidationErrorMessages);
					} else {
						Flash["Msg"] = "Error :" + ex.Message + " " + comment.Length.ToString();
					}
					RedirectToReferrer ();
					return;
				}
			}

			PropertyBag["to"] = ConfigurationManager.AppSettings["mailadmin"];
			PropertyBag["cc"] = purchase.UserRequest + "@" + ConfigurationManager.AppSettings["defaultdomain"];
			PropertyBag["from"] = ConfigurationManager.AppSettings["mailadmin"];
			PropertyBag["subject"] = "[compras] Compra recepcionada "+ purchase.Status +" ID:" + purchase.Id.ToString ();
			PropertyBag["purchase"] = purchase;
			PropertyBag["purchaseproducts"] = purchase.PurchaseProducts;
			DeliverEmail(RenderMailMessage ("close",null,PropertyBag));

			RenderView ("approval");
		}

		public void GetUpload(string name, string path) {
			FileStream fs = new FileStream("../upload_compras/" + path, FileMode.Open);
			

			Context.Response.ContentType = "application/octet-stream";
			Context.Response.BinaryWrite(fs);
			
			string encodedFilename = new UrlHelper(Context).UrlEncode(name);
			Context.Response.AppendHeader("Content-Disposition", String.Format("attachment; filename={0}", encodedFilename));
			CancelLayout();
			CancelView();
			return;
		}
	}
}

