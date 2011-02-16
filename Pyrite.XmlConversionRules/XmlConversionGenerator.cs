using System;
using System.Xml.Linq;
using Pyrite.DataLayer;

namespace Pyrite.XmlConversionRules
{
	public class XmlConversionGenerator
	{
		private IDatabaseAdapter adapter;
		private static readonly string Version = "1.0";
		private static readonly string Encoding = "utf-8";
		private static readonly string Standalone = "yes";

		public XmlConversionGenerator(IDatabaseAdapter adapter)
		{
			this.adapter = adapter;
		}

		public ConversionDocument GenerateXmlConversionTemplate(string table)
		{
			var fields = this.adapter.GetSchema(table);

			var fieldRename = new XElement("FieldRename");
			foreach (var field in fields)
				fieldRename.Add(new XElement("Field", new object[] { new XAttribute("source", field), field }));

			var extractColumn = new XElement("ExtractColumn",
				new XElement("Field", new object[] { new XAttribute("source", "[source field]"), "[dest table]" }));

			var extractAssoc = new XElement("ExtractAssociation",
				new XElement("ExtractedTable",
					new object[] { new XAttribute("name", "[extract table]"), new XElement("Field", "[Extracted Field]") }));

			var doc = new XDocument(
				new XDeclaration(Version, Encoding, Standalone),
				new XElement("DBConversionRule",
					new object[] { 
						new XAttribute("source_table", table),
						new XAttribute("dest_table", "[dest table]"),
						fieldRename, 
						extractColumn, 
						extractAssoc}));
				
			return new ConversionDocument(doc);
		}

	}
}
