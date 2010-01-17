using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace aspmvc
{
	/// <summary>
	/// Summary description for Error.
	/// </summary>
	public class Error : System.Web.UI.Page
	{
		/// <summary>
		/// Summary description for ErrorMsg.
		/// </summary>
		protected System.Web.UI.WebControls.Label ErrorMsg;
	
		/// <summary>
		/// Summary description for Page_Load.
		/// </summary>
		private void Page_Load(object sender, System.EventArgs e)
		{
			ErrorMsg.Text = "Error";
		}

		#region Web Form Designer generated code
		/// <summary>
		/// Summary description for OnInit.
		/// </summary>
		override protected void OnInit(EventArgs e)
		{
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
