using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RavenDB.AspNet.Identity
{
	internal static class Util
	{
		public static string ToHex(byte[] bytes)
		{
			StringBuilder sb = new StringBuilder(bytes.Length*2);
			for (int i = 0; i < bytes.Length; i++)
				sb.Append(bytes[i].ToString("x2"));
			return sb.ToString();
		}

		public static byte[] FromHex(string hex)
		{
			if (hex == null)
				throw new ArgumentNullException("hex");
			if (hex.Length%2 != 0)
				throw new ArgumentException("Hex string must be an even number of characters to convert to bytes.");

			byte[] bytes = new byte[hex.Length/2];

			for (int i = 0, b = 0; i < hex.Length; i += 2, b++)
				bytes[b] = Convert.ToByte(hex.Substring(i, 2), 16);

			return bytes;
		}

		public static IList<T> ToIList<T>(this IEnumerable<T> enumerable)
		{
			return enumerable.ToList();
		}
	}
}
