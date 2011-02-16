using System;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler
{
	public class TableMapperFactory
	{
		public static TableMapper Create(IDatabaseAdapter src, IDatabaseAdapter dest, string srcTableName)
		{
			ITableConversionRules rules;
			switch (srcTableName) {
				case "tblContacts":
					rules = new AccountConversionRules();
					break;
				default:
					throw new Exception(String.Format("Unknown srcTableName {0}", srcTableName));
			}

			return new TableMapper(src, dest, rules);
		}
	}
}
