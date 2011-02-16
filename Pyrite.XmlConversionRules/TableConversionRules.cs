using System;
using System.Collections.Generic;

namespace Pyrite.XmlConversionRules
{
	public class TableConversionRules : ITableConversionRules
	{
		public string SrcTableName { get; set; }
		public string DestTableName { get; set; }
		public Dictionary<string, string> FieldRenames { get; set; }
		public Dictionary<string, string> ExtractedColumns { get; set; }
		public Dictionary<string, string[]> ExtractedAssociations { get; set; }

		public Dictionary<string, string[]> InferRelations
		{
			get { throw new NotImplementedException(); }
		}
	}
}
