using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Principal;

namespace ComprasWeb.Models
{
	public class User : IPrincipal
	{
		private static List<string> _admins = new List<string>() {"jpino","logas","lmolina"};

		public User (string username) {
			UserName = username;
		}
		
		public string UserName {
			get; set;
		}

		public string Cn {
			get; set;
		}

		public string Mail {
			get; set;
		}

		#region IPrincipal Members

        public IIdentity Identity
        {
            get { return new GenericIdentity(this.UserName); }
        }

        public bool IsInRole(string role)
        {
            switch (role)
            {
                case ("admin"): {
					if (admins.Contains (UserName.ToLower ()))
                    	return true;
					else
						return false;
					}
                case ("approver"): {
					CompanyApprover[] ca = CompanyApprover.FindByLogin(UserName.ToLower ());
					if ((ca != null) && (ca.Length > 0)) 
						return true;
					else
						return false;
					}
                default:
                    return false;
            }
        }

		public static List<string> admins {
			get { return _admins; }
		}

		#endregion
	}
}

