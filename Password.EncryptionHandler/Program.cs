using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Password.EncryptionHandler
{
    class Program
    {
		static void Main(string[] args)
		{
			var passwordManager = new NetFourMembershipProvider();

			string tempdb_connection_string = ConfigurationManager.ConnectionStrings["TEMP_DB"].ConnectionString;
			SqlConnection localConn = new SqlConnection(tempdb_connection_string);
			string temporary_table = ConfigurationManager.AppSettings["TEMP_TABLE"];
			//Encrypt loaded data (uncomment new machine keys)
			Crypto(localConn, passwordManager, temporary_table);
        }
		public static void Crypto(SqlConnection local, NetFourMembershipProvider passwordManager, string temp_table)
		{
			SqlCommand selectCommand;
			SqlCommand insertCommand;
			SqlDataReader dataReader;
			string selectText = @"SELECT [OldClearText], [Salt], [UserId] FROM " + temp_table;
			try
			{
				local.Open();
				selectCommand = new SqlCommand(selectText, local);
				dataReader = selectCommand.ExecuteReader();
				while (dataReader.Read())
				{
					string plainText = dataReader.GetValue(0).ToString();
					string salt = dataReader.GetValue(1).ToString();
					string uid = dataReader.GetValue(2).ToString();
					string newCipher = passwordManager.GetEncryptedPassword(plainText, salt);
					string newPlain = passwordManager.GetClearTextPassword(newCipher);
					string insertText = $"UPDATE " + temp_table + $" SET [NewCipherText] = \'{newCipher}\', [NewClearText] = \'{newPlain.Replace(@"'", @"''")}\' WHERE [UserId] = \'{uid}\'";
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
