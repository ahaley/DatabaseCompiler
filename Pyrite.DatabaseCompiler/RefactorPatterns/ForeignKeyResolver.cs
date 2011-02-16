using System;
using System.Collections.Generic;
using System.Linq;

namespace Pyrite.DatabaseCompiler.RefactorPatterns
{
	public class ForeignKeyResolver
	{
		private Dictionary<string, ExtractedColumn> extractedColumns = new Dictionary<string, ExtractedColumn>();

		private class ExtractedColumn
		{
			private readonly Dictionary<string, int> fieldToId = new Dictionary<string, int>();
			
			public int GetId(string field)
			{
				return this.fieldToId.Keys.Contains(field) ? this.fieldToId[field] : -1;
			}	

			public void InsertField(int id, string field)
			{
				this.fieldToId.Add(field, id);
			}
		}

		public void InsertID(string extractedTable, object field, int id)
		{
			if (!this.extractedColumns.Keys.Contains(extractedTable))
				this.extractedColumns.Add(extractedTable, new ExtractedColumn());
			this.extractedColumns[extractedTable].InsertField(id, field.ToString());
		}

		public int GetForeignKey(string extractedTable, object value)
		{
			if (!this.extractedColumns.Keys.Contains(extractedTable))
				return -1;
			return this.extractedColumns[extractedTable].GetId(value.ToString());
		}

	}
}
