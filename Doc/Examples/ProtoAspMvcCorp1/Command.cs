using System;
using System.Web;

public interface Command
{
	void Execute(HttpContext context);
}
