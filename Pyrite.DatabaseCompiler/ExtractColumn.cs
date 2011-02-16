using System;
using System.Collections.Generic;
using System.Data;

namespace Pyrite.DatabaseCompiler
{
	/// <summary>
	/// This class handles the Extract Column database refactor between src and dest.
	/// </summary>
	public class ExtractColumn
	{
		private readonly IDatabaseAdapter _src;
		private readonly IDatabaseAdapter _dest;
		private readonly ITableConversionRules _rules;
		private readonly SqlGenerator _destGenerator;
		private readonly List<ExtractedColumn> _extractedColumns;

		public ExtractColumn(IDbConnection src, IDbConnection dest, ITableConversionRules rules) :
			this(new DatabaseAdapter(src), new DatabaseAdapter(dest), rules)
		{
		}

		public ExtractColumn(IDatabaseAdapter src, IDatabaseAdapter dest, ITableConversionRules rules)
		{
			_src = src;
			_dest = dest;
			_rules = rules;
			_destGenerator = new SqlGenerator(_dest);
			_extractedColumns = new List<ExtractedColumn>();
		}

		public ExtractedColumn[] ExtractedColumns { get { return _extractedColumns.ToArray(); } }

		/// <summary>
		/// This method populates the extracted column using the information stored in the rules member.
		/// </summary>
		/// <returns>The total number of rows inserted throughout all of the extracted columns.</returns>
		public int InsertExtractedColumns()
		{
			var affected = 0;
			foreach (var col in _rules.ExtractedColumns.Keys) {
				var colValues = _src.ExtractColumnDistinct(_rules.SrcTableName, col);
				affected += InsertColumnsIntoDest(col, colValues);
			}
			return affected;
		}

		private int InsertColumnsIntoDest(string col, object[] colValues)
		{
			var extractedColumn = new ExtractedColumn(col);
			var affected = 0;
			foreach (var colValue in colValues) {
				if (DBNull.Value == colValue) continue;
				var id = InsertColumnIntoDest(col, colValue);
				extractedColumn.InsertField(id, colValue.ToString());
				affected++;
			}
			_extractedColumns.Add(extractedColumn);
			return affected;
		}

		private int InsertColumnIntoDest(string col, object colValue)
		{
			var insert = _destGenerator.GenerateInsert(
				_rules.ExtractedColumns[col],
				new Dictionary<string, object> { { "name", colValue } });
			_dest.ExecuteNonQuery(insert);
				
			return _dest.GetIdLastInsert();
		}

	}
}

