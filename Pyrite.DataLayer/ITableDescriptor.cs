using System;

namespace Pyrite.DataLayer
{
	public interface ITableDescriptor
	{
		string[] GetFields(string tableName);
	}
}
