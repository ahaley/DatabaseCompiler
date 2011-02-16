using System;
using System.Collections.Generic;
using System.Linq;
using Pyrite.DatabaseCompiler.RefactorPatterns;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler
{
	
	/// <summary>
	/// Responsible for mapping a record from a database table into an insert statement for 
	/// another database table. By default RecordMapper will assume that the field names
	/// for the two tables match the source table fields. It is up to the programmer to 
	/// modify the generated xml file with the proper name mappings to the destination
	/// table as well as the proper destination table name.
	/// </summary>
	public class RecordMapper : IRecordMapper
	{
		private readonly ITableConversionRules rules;
		private readonly ForeignKeyResolver foreignKeyResolver;

		public RecordMapper(ITableConversionRules rules) :
			this(rules, null)
		{
		}

		public RecordMapper(ITableConversionRules rules, ForeignKeyResolver foreignKeyResolver)		
		{
			this.rules = rules;
			this.foreignKeyResolver = foreignKeyResolver;
		}

		/// <summary>
		/// Converts a record from one database table to a record of another database
		/// according to the rules defined in the rules member. The records are a mapping
		/// of column names to value objects.
		/// </summary>
		/// <param name="record">A record from the source database table.</param>
		/// <returns>A record of the destination database table.</returns>
		public Dictionary<string, object> MapRecord(Dictionary<string, object> record)
		{
			var mappedRecord = new Dictionary<string, object>();
			foreach (var field in record) {
				if (null == rules)
					mappedRecord.Add(field.Key, field.Value);
				else if (rules.FieldRenames.Keys.Contains(field.Key))
					mappedRecord.Add(rules.FieldRenames[field.Key], field.Value);
				else if (rules.ExtractedColumns.Keys.Contains(field.Key) && null != foreignKeyResolver) {
					MapExtractedColumn(mappedRecord, field.Key, field.Value);
				}
			}
			return mappedRecord;
		}

		private void MapExtractedColumn(Dictionary<string, object> mappedRecord, string fieldname, object value)
		{
			var extractedTable = rules.ExtractedColumns[fieldname];
			var mappedColumn = String.Format("{0}_id", extractedTable.ToLower());
			var mappedValue = foreignKeyResolver.GetForeignKey(extractedTable, value);
			if (mappedValue != -1)
				mappedRecord.Add(mappedColumn, mappedValue);
		}
	}
}
