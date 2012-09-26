using System;
using System.Configuration;
using System.Web;
using System.Web.Security;
using Castle.MonoRail.Framework;
using log4net;
using System.DirectoryServices;


using ComprasWeb.Models;

namespace ComprasWeb.Controllers
{
	[Rescue("generalerror")]
	public class LoginController : SmartDispatcherController
	{
		private ILog log = LogManager.GetLogger ("Login");
		public void index() {
		}

		public void Login (string userName, string passwd, string returnUrl) {
			string path = ConfigurationManager.AppSettings["pathpdc"];

			if (estaAutenticado(ConfigurationManager.AppSettings["domain"], userName, passwd, path)) {
					if (Session.Contains ("jumpTo")) {
						string jumpTo = (string) Session["jumpTo"];                
						RedirectToUrl (jumpTo);
					} else
						Redirect ("purchase","new");
			}
			else {
				Flash["TypeMsg"] = "alert alert-error";
				Flash["Msg"] = "Error al autenticar";
				RedirectToReferrer();
			}
		}

		private bool estaAutenticado(string dominio, string usuario, string pwd, string path) {
			//Armamos la cadena completa de dominio y usuario
			string domainAndUsername = dominio + @"\" + usuario;
			//string domainAndUsername = usuario;
			//Creamos un objeto DirectoryEntry al cual le pasamos el URL, dominio/usuario y la contraseña
			DirectoryEntry entry = new DirectoryEntry(path, domainAndUsername, pwd);
			try
			{
				DirectorySearcher search = new DirectorySearcher(entry);
				search.Filter = "(&(sAMAccountName="+ usuario +"))";
				SearchResult result = search.FindOne();
				if (result == null) {
					log.Warn("User failed by pass in: " + usuario);
					return false;
				}
				else {
					User user = new User(usuario);

					if (string.IsNullOrEmpty(result.Properties["mail"][0].ToString ()))
						user.Mail = usuario.ToLower () + "@" + ConfigurationManager.AppSettings["defaultdomain"];
					else
						user.Mail = result.Properties["mail"][0].ToString ();

					if (string.IsNullOrEmpty(result.Properties["cn"][0].ToString ()))
						user.Cn = usuario;
					else
						user.Cn = result.Properties["cn"][0].ToString ();



					Context.CurrentUser = user;
					//Context.Session.Clear();
					Context.Session.Add("user",user);
					//Context.Session.Add("IsAuthorized",true);
					HttpCookie cookie = new HttpCookie("user", user.Identity.Name);
					cookie.Expires = DateTime.Now.AddHours(Convert.ToInt32(ConfigurationManager.AppSettings["timecookie"]));
					HttpContext.Response.SetCookie(cookie);

					log.Debug("User logged in: "+ usuario);
					return true;
				}
			}
			catch (Exception ex) {
				log.Error("User failed in: " + usuario);
				Flash["TypeMsg"] = "alert alert-error";
				Flash["Msg"] = "Error al autenticar : " + ex.Message;
				return false;
			}
		}
		
		public void LogOut() {
			Session.Clear();
			HttpCookie cookie = HttpContext.Response.Cookies.Get("user");
			cookie.Expires = DateTime.Now.AddMinutes(-1);
			HttpContext.Response.Cookies.Set(cookie);
			Redirect("login","index");
		}
	}
}

