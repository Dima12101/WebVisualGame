using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text;
using System.Security.Cryptography;

namespace WebVisualGame
{
	public static class SignGenerator
	{
		public static string GetSign(string key)
		{
			var provider = new MD5CryptoServiceProvider();

			byte[] hash = provider.ComputeHash(Encoding.Default.GetBytes(key));

			return BitConverter.ToString(hash).ToLower().Replace("-", "");
		}
	}
}
