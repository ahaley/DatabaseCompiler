using System.Collections.Generic;
using System.Linq;
using System.Data.Common;
using System.Data;

namespace Pyrite.DataLayer
{
	public class TableDescriptor : ITableDescriptor
	{
		private readonly DbConnection conn;
		private readonly Dictionary<string, List<string>> tableFields = new Dictionary<string, List<string>>();

		public TableDescriptor(DbConnection conn)
		{
			this.conn = conn;
		}

		public string[] GetFields(string tableName)
		{
			var schema = conn.GetSchema("Columns",
				new[] { null, null, tableName, null });

			if (!tableFields.Keys.Contains(tableName)) {
				var fields = new List<string>();
				foreach (DataRow row in schema.Rows)
					fields.Add(row["COLUMN_NAME"].ToString());
				tableFields.Add(tableName, fields);
			}
			return tableFields[tableName].ToArray();
		}
		
	}
}
