using System;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace aspmvc
{
	/// <summary>
	/// Summary description for CIndex.
	/// </summary>
	public class CIndex : BasePage
	{
		/// <summary>
		/// Summary description for OnInit.
		/// </summary>
		override protected void OnInit(EventArgs e)
		{
			Response.Redirect("ListView.aspx"); 
		}
	}
}
