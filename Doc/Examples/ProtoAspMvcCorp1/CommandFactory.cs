using System;
using System.Collections.Specialized;

public class CommandFactory
{
	public static Command Make(NameValueCollection parms)
	{
		string sView = parms["view"];

		Command command = null;

		if(sView == null || sView.Equals("default")|| sView.Equals("index"))
			command = new CCommandIndex();
		return command;
	}
}
