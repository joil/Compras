using System;
using System.DirectoryServices;

using ComprasWeb.Models;

namespace ComprasWeb.Controllers
{
	public class Utilities
	{
		/// <summary>
		/// Get the validation error text from the validationErrors array
		/// </summary>
		/// <param name="validationErrors"></param>
		/// <returns></returns>
		public static string GetValidationErrorText(string[] validationErrors)
		{
			string validationTextToDisplay = string.Empty;
			for (int i = 0; i < validationErrors.Length; i++)
			{
				validationTextToDisplay += validationErrors[i].ToString() + "<br />";
			}
			return validationTextToDisplay;
		}

		public static string GetMailByLogin(string Login)
		{
			SearchResult result;
			string path = "ldap://vtrnc04pdc01/dc=vtholding,dc=cl";
			string domainAndUsername = "vtholding" + @"\traspaso";
			try {
				DirectoryEntry entry = new DirectoryEntry(path, domainAndUsername, "Rancagua2008");
				DirectorySearcher search = new DirectorySearcher(entry);
				search.Filter = "(&(sAMAccountName="+ Login +"))";
				result = search.FindOne();
			}
			catch (Exception ex) {
				throw (ex);
			}

			return result.Properties["mail"][0].ToString ();
		}

		public static User GetUserByCookie(string name) {
			User user = new User(name);
			SearchResult result;
			string path = "ldap://vtrnc04pdc01/dc=vtholding,dc=cl";
			string domainAndUsername = "vtholding" + @"\traspaso";
			try {
				DirectoryEntry entry = new DirectoryEntry(path, domainAndUsername, "Rancagua2008");
				DirectorySearcher search = new DirectorySearcher(entry);
				search.Filter = "(&(sAMAccountName="+ name +"))";
				result = search.FindOne();
				user.Cn = result.Properties["cn"][0].ToString ();
				user.Mail = result.Properties["mail"][0].ToString ();
			}
			catch (Exception ex) {
				throw (ex);
			}
			return user;
		}
	}
}

