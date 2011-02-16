namespace Pyrite.DataLayer
{
	public interface ISqlGeneratorFactory
	{
		ISqlGenerator GetSqlGenerator(IDatabaseAdapter adapter);
	}
}
