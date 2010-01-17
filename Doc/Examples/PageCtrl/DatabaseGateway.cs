using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

public class DatabaseGateway
{
	public static string RetrieveAddress(string name)
	{
		String address = null;

		String selectCmd = 
			String.Format("select * from webuser where (id = '{0}')",
			name);

		SqlConnection myConnection = 
			new SqlConnection("server=(local);database=webusers;Trusted_Connection=yes");
		SqlDataAdapter myCommand = new SqlDataAdapter(selectCmd, myConnection);

		DataSet ds = new DataSet();
		myCommand.Fill(ds,"webuser");
		if(ds.Tables["webuser"].Rows.Count == 1)
		{
			DataRow row = ds.Tables["webuser"].Rows[0];
			address = row["address"].ToString();
		}

		return address;
	}
}
