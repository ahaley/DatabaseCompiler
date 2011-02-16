using System;

namespace Pyrite.DataLayer
{
	class SqlGeneratorFactory : ISqlGeneratorFactory
	{
		public ISqlGenerator GetSqlGenerator(IDatabaseAdapter adapter)
		{
			return new SqlGenerator(adapter);
		}
	}
}
