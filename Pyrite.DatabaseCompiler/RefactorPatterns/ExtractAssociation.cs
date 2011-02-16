using System.Collections.Generic;
using System.Linq;
using System;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler.RefactorPatterns
{
	/// <summary>
	/// Database refactoring pattern that extracts a many to many relation from a main table
	/// by populating an extracted table and associating it back to the main table through
	/// an association table. The main table should already be populated and be accessible
	/// through src.
	/// </summary>
	public class ExtractAssociation : IRefactorPattern
	{
		private readonly IDatabaseAdapter src;
		private readonly IDatabaseAdapter dest;
		private readonly ITableConversionRules rules;
		private readonly Dictionary<string, int> assocMapping = new Dictionary<string, int>();
		private const string Name = "name";

		public ExtractAssociation(IDatabaseAdapter src, IDatabaseAdapter dest, ITableConversionRules rules)
		{
			this.src = src;
			this.dest = dest;
			this.rules = rules;
			this.ExtractedAssociation = new ExtractedAssocation();
		}

		public ExtractedAssocation ExtractedAssociation { get; private set; }

		/// <summary>
		/// Extracts the columns out of the source table for each of the extracted
		/// associations defined in rules. The extracted tables needs to be populated
		/// with the unique values from all these columns specific to that extracted
		/// table. A structure needs to be stored that contains these mappings to
		/// populate the association table that links the destination table and the
		/// extracted table that is created here.
		/// </summary>
		/// <returns>The number of records inserted into extracted tables</returns>
		public int PerformRefactor()
		{
			var records = new Queue<Dictionary<string, object>>();
			Dictionary<string, object> record;

			while (null != (record = this.src.ExtractRecord(this.rules.SrcTableName))) {
				records.Enqueue(record);
			}

			var count = 0;
			foreach (var extractedTable in this.rules.ExtractedAssociations.Keys.ToList()) {
				var fields = this.rules.ExtractedAssociations[extractedTable];
				var values = GetUniques(records, fields);
				foreach (var value in values) {
					count += this.dest.InsertRecord(extractedTable,
						new Dictionary<string, object> { { "name", value } });
					var id = this.dest.GetIdLastInsert();
					this.ExtractedAssociation.InsertExtractedField(extractedTable, id, value);
				}
			}
			return count;
		}

		public void InsertAssociationInserts(Dictionary<string, object> record, int id)
		{
			var tableId = this.rules.DestTableName.ToLower() + "_id";
			if (null == this.rules.ExtractedAssociations)
				return;
			foreach (var associatedTable in this.rules.ExtractedAssociations.Keys) {
				var associatedTableId = associatedTable.ToLower() + "_id";
				var associationTableName = this.rules.DestTableName + "To" + associatedTable;
				foreach (var value in this.rules.ExtractedAssociations[associatedTable]) {
					if (!record.Keys.Contains(value) || null == record[value] || "*" == record[value].ToString())
						continue;
					var associationInsert = new Dictionary<string, object>();
					associationInsert[tableId] = id;
					associationInsert[associatedTableId] = this.ExtractedAssociation.GetIdFromField(associatedTable, record[value]);
					this.dest.InsertRecord(associationTableName, associationInsert);
				}
			}
		}

		public static object[] GetUniques(Queue<Dictionary<string, object>> records, string[] fields)
		{
			var list = new List<object>();
			foreach (var record in records) {
				if (null == record)
					continue;
				foreach (var pair in record) {
					if (null == pair.Value)
						continue;
					if (fields.Contains(pair.Key) && pair.Value.ToString() != "*" && !list.Contains(pair.Value)) {
						list.Add(pair.Value);
					}
				}
			}
			return list.ToArray();
		}

	}
}
