using System;
using System.Collections.Specialized;
using System.Configuration;
using System.Xml;
using System.Web;

public class UrlMap : IConfigurationSectionHandler 
{
	private readonly NameValueCollection _commands = new NameValueCollection();

	public const string SECTION_NAME="controller.mapping";

	public static UrlMap SoleInstance 
	{
		get {return (UrlMap) ConfigurationSettings.GetConfig(SECTION_NAME);}
	}

	object IConfigurationSectionHandler.Create(object parent,object configContext, XmlNode section) 
	{
		return (object) new UrlMap(parent,configContext, section);   
	}

	private UrlMap() {/*no-op*/}

	public UrlMap(object parent,object configContext, XmlNode section) 
	{
		try 
		{
			XmlElement entriesElement = section["entries"];
			foreach(XmlElement element in entriesElement) 
			{
				_commands.Add(element.Attributes["key"].Value,element.Attributes["url"].Value);
			}
		} 
		catch (Exception ex) 
		{
			throw new ConfigurationException("Error while parsing configuration section.",ex,section);
		}
	}

	public NameValueCollection Map
	{
		get { return _commands; }
	}
}
