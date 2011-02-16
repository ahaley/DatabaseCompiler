using System;

namespace Pyrite.DatabaseCompiler
{
	public interface ITableDescriptor
	{
		string[] GetFields(string tableName);
	}
}
