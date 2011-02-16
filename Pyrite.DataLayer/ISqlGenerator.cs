using System.Collections.Generic;

namespace Pyrite.DataLayer
{
	public interface ISqlGenerator
	{
		string[] GetFields(string tableName);
		string GenerateSelect(string tableName);
		string GenerateSelect(string tableName, string where);
		string GenerateInsert(string tableName);
		string GenerateInsert(string tableName, Dictionary<string, object> record);
	}
}