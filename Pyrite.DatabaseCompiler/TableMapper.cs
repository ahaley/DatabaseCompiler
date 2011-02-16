using System;
using System.Collections.Generic;
using Pyrite.DatabaseCompiler.RefactorPatterns;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler
{
	public class TableMapper
	{	
		private readonly IDatabaseAdapter src;
		private readonly IDatabaseAdapter dest;
		private readonly ITableConversionRules rules;
		private ExtractAssociation extractAssociation;

		public TableMapper(IDatabaseAdapter src, IDatabaseAdapter dest, ITableConversionRules rules)
		{
			this.src = src;
			this.dest = dest;
			this.rules = rules;
		}

		public void MapTable()
		{
			var extractColumns = new ExtractColumn(src, dest, rules);
			extractColumns.PerformRefactor();

			extractAssociation = new ExtractAssociation(src, dest, rules);
			extractAssociation.PerformRefactor();

			var mapper = new RecordMapper(
				rules,
				extractColumns.ForeignKeyResolver);

			InsertMappedFields(mapper);
		}

		public int InsertMappedFields()
		{
			return InsertMappedFields(new RecordMapper(rules));	
		}

		/// <summary>
		/// Maps and inserts all records from the source table into the destination
		/// table. The source and destination table are specified in the rules member
		/// variable. If the extract association pattern was called then it will also
		/// insert another table into the assocation table for each record.
		/// </summary>
		/// <param name="mapper"></param>
		/// <returns></returns>
		public int InsertMappedFields(IRecordMapper mapper)
		{
			var affected = 0;
			Dictionary<string, object> record;
			while (null != (record = this.src.ExtractRecord(rules.SrcTableName))) {
				affected += this.dest.InsertRecord(
					this.rules.DestTableName, 
					mapper.MapRecord(record));
				var id = this.dest.GetIdLastInsert();

				if (null != this.extractAssociation)
					this.extractAssociation.InsertAssociationInserts(record, id);
			}
			return affected;
		}

	}
}
