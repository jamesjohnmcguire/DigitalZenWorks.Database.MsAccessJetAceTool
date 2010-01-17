using System;
using System.Web.UI;
using System.Web.UI.WebControls;

public class BasePage : Page
{
	protected Label eMail;
	protected Label siteName;

	virtual protected void PageLoadEvent(object sender, System.EventArgs e)
	{}

	protected void Page_Load(object sender, System.EventArgs e)
	{
		if(!IsPostBack)
		{
			eMail.Text = (string)Context.Items["address"];
			siteName.Text = (string)Context.Items["site"];
			PageLoadEvent(sender, e);
		}
	}

	#region Web Form Designer generated code
	#endregion
}
