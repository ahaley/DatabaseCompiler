using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System;

namespace Pyrite.DataLayer
{
	/// <summary>
	/// This class generates sql statements for a given DbConnection wrapped in a TableDescriptor.
	/// </summary>
	public class SqlGenerator : ISqlGenerator
	{
		private readonly TableDescriptor descriptor;

		public SqlGenerator(IDatabaseAdapter adapter) :
			this(adapter.DbConnection)
		{
		}

		public SqlGenerator(IDbConnection conn) :
			this(new TableDescriptor(conn as DbConnection))
		{
		}

		private SqlGenerator(TableDescriptor descriptor)
		{
			this.descriptor = descriptor;
		}

		public string[] GetFields(string tableName)
		{
			return this.descriptor.GetFields(tableName);
		}

		public string GenerateSelect(string tableName)
		{
			return GenerateSelect(tableName, null);
		}

		public string GenerateSelect(string tableName, string where)
		{
			var fields = this.descriptor.GetFields(tableName);
			var select = string.Format("SELECT {0} FROM {1}",
				string.Join(", ", fields),
				tableName);
			if (null != where)
				select += String.Format(" WHERE {0}", where);
			return select;
		}

		public string GenerateInsert(string tableName)
		{
			return GenerateInsert(tableName, null);
		}

		public string GenerateInsert(string tableName, Dictionary<string, object> record)
		{
			var fields = record.Keys.ToArray();
			var values = record.Values.ToArray();

			return string.Format("INSERT INTO {0} ({1}) VALUES({2})",
				tableName,
				string.Join(", ", fields),
				string.Join(", ", TypeResolver.FormatValues(values)));
		}

	}
}
