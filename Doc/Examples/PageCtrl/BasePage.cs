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
			string name = Context.User.Identity.Name;

			eMail.Text = DatabaseGateway.RetrieveAddress(name);
			siteName.Text = "Micro-site";

			PageLoadEvent(sender, e);
		}
	}


	#region Web Form Designer generated code
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
