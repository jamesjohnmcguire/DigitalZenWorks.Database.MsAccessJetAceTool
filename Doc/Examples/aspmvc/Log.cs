using System;
using System.Diagnostics;
using System.Globalization; 
using System.IO;

namespace aspmvc
{
	/// <summary>
	/// 
	/// </summary>
	public class Log
	{
		/// <summary>
		/// Summary description for OnInit.
		/// </summary>
		protected	bool			m_bKeepOpen		= false;
		/// <summary>
		/// Summary description for OnInit.
		/// </summary>
		protected	bool			m_bDebug		= false;
		/// <summary>
		/// Summary description for OnInit.
		/// </summary>
		protected	string			m_sLogFileName	= string.Empty;
		/// <summary>
		/// Summary description for OnInit.
		/// </summary>
		protected	FileStream		m_FileStream	= null;
		/// <summary>
		/// Summary description for m_StreamWriter.
		/// </summary>
		protected	StreamWriter	m_StreamWriter	= null;

		/// <summary>
		/// Summary description for Log.
		/// </summary>
		public Log(string sLogFileName, bool bKeepOpen)
		{
			m_bKeepOpen	= bKeepOpen;
			m_sLogFileName	= sLogFileName;
		}
		/// <summary>
		/// Summary description for Add.
		/// </summary>
		public void Add(string sEvent)
		{
			try
			{
				if (m_StreamWriter == null)
				{
					if (m_FileStream == null)
						//set up a filestream
						m_FileStream	= new FileStream(@m_sLogFileName, FileMode.OpenOrCreate, FileAccess.Write);

					//set up a streamwriter for adding text
					m_StreamWriter	= new StreamWriter(m_FileStream);
				}

				DateTime	timeNow		= DateTime.Now;
				string		tTimeNow	= timeNow.ToString( "u", DateTimeFormatInfo.InvariantInfo);

				//find the end of the underlying filestream
				m_StreamWriter.BaseStream.Seek(0, SeekOrigin.End);

				//add the text 
				m_StreamWriter.WriteLine(tTimeNow + " - " + sEvent);
				m_StreamWriter.Flush();

				if (!m_bKeepOpen)
					m_StreamWriter.Close();
			}
			catch
			{
				Debug.Assert( m_FileStream != null, "Log class: m_FileStream = null Event: " + sEvent);
				Debug.Assert( m_StreamWriter != null, "Log class: m_StreamWriter = null Event: " + sEvent);
			}
		}
	}
}
