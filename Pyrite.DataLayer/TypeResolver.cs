using System;
using System.Collections.Generic;

namespace Pyrite.DataLayer
{
	public class TypeResolver
	{
		public static string[] FormatValues(object[] values)
		{
			var formattedValues = new List<string>();
			foreach (var value in values) {
				formattedValues.Add(FormatValue(value));
			}
			return formattedValues.ToArray();
		}

		public static string FormatValue(object value)
		{
			if (value is String)
				return string.Format("'{0}'", EscapeText(value as string));
			if (value is Int32)
				return value.ToString();
			if (value is DateTime)
				return string.Format("'{0}'", value);
			if (value is Boolean)
				return (bool)value ? "1" : "0";
			if (value is DBNull)
				return "";
			throw new Exception(String.Format("Unknown value type: {0}", value.GetType()));
		}

		private static string EscapeText(string raw)
		{
			return raw.Replace("'", "''");
		}

	}
}
