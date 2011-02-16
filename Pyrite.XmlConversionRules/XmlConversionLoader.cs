using System;
using System.Xml.Linq;
using System.Collections.Generic;

namespace Pyrite.XmlConversionRules
{
	public class XmlConversionLoader
	{
		public static ITableConversionRules LoadConversionDocument(XDocument doc)
		{
			var rules = new TableConversionRules();

			var dBConversionRuleNode = doc.Element("DBConversionRule");
			rules.SrcTableName = dBConversionRuleNode.Attribute("source_table").Value;
			rules.DestTableName = dBConversionRuleNode.Attribute("dest_table").Value;

			rules.FieldRenames = GetFieldRenames(dBConversionRuleNode);
			rules.ExtractedColumns = GetExtractColumns(dBConversionRuleNode);
			rules.ExtractedAssociations = GetExtractAssociations(dBConversionRuleNode);

			return rules;
		}

		private static Dictionary<string, string> GetFieldRenames(XElement dBConversionRuleNode)
		{
			var fieldRenameNode = dBConversionRuleNode.Element("FieldRename");
			var fieldRenames = new Dictionary<string, string>();
			foreach (var desc in fieldRenameNode.Descendants())
				fieldRenames.Add(desc.Attribute("source").Value, desc.Value);
			return fieldRenames;
		}

		private static Dictionary<string, string> GetExtractColumns(XElement dBConversionRuleNode)
		{
			var extractColumnNode = dBConversionRuleNode.Element("ExtractColumn");
			var extractColumns = new Dictionary<string, string>();
			foreach (var desc in extractColumnNode.Descendants())
				extractColumns.Add(desc.Attribute("source").Value, desc.Value);
			return extractColumns;
		}

		private static Dictionary<string, string[]> GetExtractAssociations(XElement dBConversionRuleNode)
		{
			var extractAssocationNode = dBConversionRuleNode.Element("ExtractAssociation");
			var extractAssociations = new Dictionary<string, string[]>();
			foreach (var extractTableNode in extractAssocationNode.Elements()) {
				var fields = new List<string>();
				foreach (var fieldNode in extractTableNode.Elements())
					fields.Add(fieldNode.Value);
				extractAssociations.Add(extractTableNode.Attribute("name").Value, fields.ToArray());
			}
			return extractAssociations;
		}
	}
}
