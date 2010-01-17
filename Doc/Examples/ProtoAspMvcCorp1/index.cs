using System;
using System.Web;

public class CCommandIndex : RedirectingCommand
{
	protected override void OnExecute(HttpContext context)
	{
		string name = context.User.Identity.Name;

		//context.Items["address"] = 
		//	WebUsersDatabase.RetrieveAddress(name);
		context.Items["view"] = "index";
	}
}
