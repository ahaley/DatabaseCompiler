using System.Collections.Generic;
using System.Data;

namespace Pyrite.DataLayer
{
	public interface IDatabaseAdapter
	{
		IDbConnection DbConnection { get; }
		int ExecuteNonQuery(string nonQuery);
		object[] ExtractColumnDistinct(string table, string column);
		Dictionary<string, object> ExtractRecord(string tableName);
		int GetIdLastInsert();
		int InsertRecord(string tableName, Dictionary<string, object> record);
		object[] ExtractMultiColumnDistinct(string table, string[] columns);
		string[] GetSchema(string table);
	}
}
