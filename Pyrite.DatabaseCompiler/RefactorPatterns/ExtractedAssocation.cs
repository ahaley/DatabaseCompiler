using System;
using System.Collections.Generic;
using System.Linq;

namespace Pyrite.DatabaseCompiler.RefactorPatterns
{
	public class ExtractedAssocation
	{
		private readonly Dictionary<string, ExtractedTable> extractedTables = new Dictionary<string, ExtractedTable>();

		private class ExtractedTable
		{
			public ExtractedTable()
			{
				this.ExtractedFields = new Dictionary<object, int>();
			}

			public Dictionary<object, int> ExtractedFields { get; private set; }
		}

		public void InsertExtractedField(string extractedTable, int id, object field)
		{
			if (!this.extractedTables.Keys.Contains(extractedTable))
				this.extractedTables.Add(extractedTable, new ExtractedTable());
			this.extractedTables[extractedTable].ExtractedFields.Add(field, id);
		}

		public int GetIdFromField(string extractedTable, object field)
		{
			return this.extractedTables[extractedTable].ExtractedFields[field];
		}
	}
}
