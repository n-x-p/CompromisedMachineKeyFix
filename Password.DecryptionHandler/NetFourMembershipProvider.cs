using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Password.DecryptionHandler
{
	public class NetFourMembershipProvider : System.Web.Security.SqlMembershipProvider
	{
		public string GetClearTextPassword(string encryptedPwd)
		{
			byte[] encodedPassword = System.Convert.FromBase64String(encryptedPwd);
			byte[] bytes = this.DecryptPassword(encodedPassword);
			if (bytes == null)
			{
				return null;
			}
			return Encoding.Unicode.GetString(bytes, 0x10, bytes.Length - 0x10);

		}
		public string GetEncryptedPassword(string pass, string salt)
		{
			byte[] bytes = Encoding.Unicode.GetBytes(pass);
			byte[] src = Convert.FromBase64String(salt);
			byte[] dst = new byte[src.Length + bytes.Length];
			byte[] inArray = null;
			Buffer.BlockCopy(src, 0, dst, 0, src.Length);
			Buffer.BlockCopy(bytes, 0, dst, src.Length, bytes.Length);
			inArray = this.EncryptPassword(dst);
			return Convert.ToBase64String(inArray);
		}
	}
}
