using System.Collections.Generic;

namespace Pyrite.DatabaseCompiler
{
	public interface IRecordMapper
	{
		/// <summary>
		/// Converts a record from one database table to a record of another database
		/// according to the rules defined in the rules member. The records are a mapping
		/// of column names to value objects.
		/// </summary>
		/// <param name="record">A record from the source database table.</param>
		/// <returns>A record of the destination database table.</returns>
		Dictionary<string, object> MapRecord(Dictionary<string, object> record);
	}
}