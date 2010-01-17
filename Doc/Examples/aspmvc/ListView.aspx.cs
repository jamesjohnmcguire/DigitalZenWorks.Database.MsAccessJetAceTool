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
	/// Summary description for ListView.
	/// </summary>
	public class ListView : BasePage
	{
		/// <summary>
		/// 
		/// </summary>
		protected System.Web.UI.WebControls.DataGrid DataGrid1;
		/// <summary>
		/// Summary description for PageLoadEvent.
		/// </summary>
		protected override void PageLoadEvent(object sender, System.EventArgs e)
		{
			sPageTitle	= "ListView";
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

		}
		#endregion
	}
}
