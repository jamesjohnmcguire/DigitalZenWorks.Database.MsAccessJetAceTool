using System;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;

using Zenware.Diagnostics;
using Zenware.DatabaseLib;

namespace aspmvc
{
	/// <summary>
	/// Summary description for BasePage.
	/// </summary>
	public class BasePage : Page
	{
		/// <summary>
		/// Summary description for eMail.
		/// </summary>
		protected Label eMail;

		/// <summary>
		/// Summary description for siteName.
		/// </summary>
		protected Label siteName;

		/// <summary>
		/// Summary description for m_oDBLib.
		/// </summary>
		protected DatabaseLibClass m_oDBLib = null;

		/// <summary>
		/// Summary description for sPageTitle.
		/// </summary>
		protected string	sPageTitle	= string.Empty;

		/// <summary>
		/// Summary description for sView.
		/// </summary>
		protected string	sView		= string.Empty;

		/// <summary>
		/// Summary description for PageLoadEvent.
		/// </summary>
		virtual protected void PageLoadEvent(object sender, System.EventArgs e)
		{}
	   
		/// <summary>
		/// Summary description for Page_Load.
		/// </summary>
		protected void Page_Load(object sender, System.EventArgs e)
		{
			if (m_oDBLib == null)
				m_oDBLib = new DatabaseLibClass(DatabaseLibClass.DB_SQLSERVER,
											"AspMvc", 
											"prjTemplate",
											"prjTemplate",
											null,
											null);

			if (m_oDBLib == null)
				Response.Redirect("error.aspx"); 

			if(!IsPostBack)
			{
				string name = Context.User.Identity.Name;

				//eMail.Text = oDataBaseGateWay.RetrieveAddress(name);
				siteName.Text = "Micro-site";

				PageLoadEvent(sender, e);
			}
		}

		#region Web Form Designer generated code
	   
		/// <summary>
		/// Summary description for OnInit.
		/// </summary>
		override protected void OnInit(EventArgs e)
		{
			//
			//
			// CODEGEN: This call is required by the ASP.NET Web Form Designer.
			//
			InitializeComponent();
			base.OnInit(e);
		}
	         
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{    
			this.Load += new System.EventHandler(this.Page_Load);

		}
		#endregion

	}
}
