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

namespace ProtoAspMvcCorp1
{
	/// <summary>
	/// Summary description for ActualPage1.
	/// </summary>
	public class ActualPage1 : BasePage
	{
		protected System.Web.UI.WebControls.Label pageNumber;

		protected override void PageLoadEvent(object sender, System.EventArgs e)
		{
			pageNumber.Text = "1";
		}

		#region Web Form Designer generated code
		#endregion
	}
}
