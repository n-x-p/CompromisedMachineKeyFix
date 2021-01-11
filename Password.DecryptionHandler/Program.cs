using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;

namespace Password.DecryptionHandler
{
    static class Program
	{
		static void Main()
		{
			var passwordManager = new NetFourMembershipProvider();


			string tempdb_connection_string = ConfigurationManager.ConnectionStrings["TEMP_DB"].ConnectionString;
			SqlConnection localConn = new SqlConnection(tempdb_connection_string);

			string prod_connection_string = ConfigurationManager.ConnectionStrings["PROD_DB"].ConnectionString;
			SqlConnection prodConn = new SqlConnection(prod_connection_string);


			string temporary_table = ConfigurationManager.AppSettings["TEMP_TABLE"];
			string production_table = ConfigurationManager.AppSettings["PROD_TABLE"];

			//Load data into db (uncomment old machine keys)
			Load(prodConn, localConn, passwordManager, temporary_table, production_table);
        }
		
		public static void Load(SqlConnection remote, SqlConnection local, NetFourMembershipProvider passwordManager, string temp_table, string prod_table)
        {
			SqlCommand selectCommand;
			SqlCommand insertCommand;
			SqlDataReader dataReader;
			string selectText = @"SELECT [Password], [PasswordSalt], [UserId] FROM " + prod_table + " WHERE [PasswordFormat] = 2";
			try
			{
				local.Open();
				remote.Open();
				selectCommand = new SqlCommand(selectText, remote);
				dataReader = selectCommand.ExecuteReader();
				while (dataReader.Read())
				{
					string pass = dataReader.GetValue(0).ToString();
					string salt = dataReader.GetValue(1).ToString();
					string user = dataReader.GetValue(2).ToString();
					string clear = passwordManager.GetClearTextPassword(pass);
					string insertText = $"INSERT INTO " + temp_table + $" ([UserId],[OldCipherText],[OldClearText],[Salt]) VALUES(\'{user}\',\'{pass}\',\'{clear.Replace(@"'", @"''")}\',\'{salt}\')";
					insertCommand = new SqlCommand(insertText, local);
					insertCommand.ExecuteNonQuery();
					insertCommand.Dispose();
				}
				dataReader.Close();
				selectCommand.Dispose();
			}
			catch (SqlException e)
			{
				Console.WriteLine(e);
			}
		}
	}
}
