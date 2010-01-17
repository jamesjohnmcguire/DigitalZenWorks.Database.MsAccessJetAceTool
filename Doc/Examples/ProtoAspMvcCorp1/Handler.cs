using System;
using System.Web;

public class Handler : IHttpHandler
{
	public void ProcessRequest(HttpContext context) 
	{
		Command command = 
			CommandFactory.Make(context.Request.Params);
		command.Execute(context);
	}

	public bool IsReusable 
	{ 
		get { return true;} 
	}
}
