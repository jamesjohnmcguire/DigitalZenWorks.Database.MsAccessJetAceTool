using System;
using System.Web;

public abstract class RedirectingCommand : Command
{
	private UrlMap map = UrlMap.SoleInstance;

	protected abstract void OnExecute(HttpContext context);

	public void Execute(HttpContext context)
	{
		OnExecute(context);
      
		string url = String.Format("{0}?{1}",
			map.Map[context.Request.Url.AbsolutePath],
			context.Request.Url.Query);

		context.Server.Transfer(url);
	}
}
