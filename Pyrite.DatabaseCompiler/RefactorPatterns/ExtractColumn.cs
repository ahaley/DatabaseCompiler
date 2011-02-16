using System;
using System.Collections.Generic;
using System.Data;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler.RefactorPatterns
{
	/// <summary>
	/// This class handles the Extract Column database refactor between src and dest.
	/// </summary>
	public class ExtractColumn : IRefactorPattern
	{
		private readonly IDatabaseAdapter src;
		private readonly IDatabaseAdapter dest;
		private readonly ITableConversionRules rules;
		private readonly SqlGenerator destGenerator;

		public ExtractColumn(IDbConnection src, IDbConnection dest, ITableConversionRules rules) :
			this(new DatabaseAdapter(src), new DatabaseAdapter(dest), rules)
		{
		}

		public ExtractColumn(IDatabaseAdapter src, IDatabaseAdapter dest, ITableConversionRules rules)
		{
			this.src = src;
			this.dest = dest;
			this.rules = rules;
			this.destGenerator = new SqlGenerator(dest);
			this.ForeignKeyResolver = new ForeignKeyResolver();
		}

		public ForeignKeyResolver ForeignKeyResolver { get; private set; }

		/// <summary>
		/// This method populates the extracted column using the information stored in the rules member.
		/// </summary>
		/// <returns>The total number of rows inserted throughout all of the extracted columns.</returns>
		public int PerformRefactor()
		{
			var affected = 0;
			foreach (var sourceColumn in this.rules.ExtractedColumns.Keys) {
				var colValues = this.src.ExtractColumnDistinct(this.rules.SrcTableName, sourceColumn);
				var extractedTable = this.rules.ExtractedColumns[sourceColumn];
				affected += InsertColumnsIntoDest(extractedTable, colValues);
			}
			return affected;
		}

		private int InsertColumnsIntoDest(string extractedTable, object[] colValues)
		{
			var affected = 0;
			foreach (var colValue in colValues) {
				if (DBNull.Value == colValue) continue;
				this.dest.InsertRecord(
					extractedTable,
					new Dictionary<string, object> { { "name", colValue } });
				var id = this.dest.GetIdLastInsert();
				this.ForeignKeyResolver.InsertID(extractedTable, colValue, id);
				affected++;
			}
			return affected;
		}

	}
}