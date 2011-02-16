using System.Collections.Generic;

namespace Pyrite.XmlConversionRules
{
	public interface ITableConversionRules
	{
		string SrcTableName { get; }
		string DestTableName { get; }
		Dictionary<string, string> FieldRenames { get; }
		Dictionary<string, string> ExtractedColumns { get; }
		Dictionary<string, string[]> InferRelations { get; }
		Dictionary<string, string[]> ExtractedAssociations { get; }
	}
}
