using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

public class SimplePage : System.Web.UI.Page
{
	protected System.Web.UI.WebControls.TextBox name;
	protected System.Web.UI.WebControls.Button MyButton;
	protected System.Web.UI.HtmlControls.HtmlGenericControl mySpan;

	public void SubmitBtn_Click(Object sender, EventArgs e) 
	{
		mySpan.InnerHtml = "Hello, " + name.Text + "."; 
	}
}
