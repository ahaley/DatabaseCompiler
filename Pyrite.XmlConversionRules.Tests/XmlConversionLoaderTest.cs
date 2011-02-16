using System;
using System.Xml.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pyrite.XmlConversionRules.Tests
{
	[TestClass]
	public class XmlConversionLoaderTest
	{
		[TestMethod]
		public void Test_LoadConversionDocument_Extracts_Source_And_Dest_Tables()
		{
			// arrange
			var xml = Properties.Resources.AssociationConversionRules;
			var document = XDocument.Parse(xml);

			// act
			var rules = XmlConversionLoader.LoadConversionDocument(document);

			// assert
			var actualRules = new AccountConversionRules();
			Assert.AreEqual(actualRules.SrcTableName, rules.SrcTableName);
			Assert.AreEqual(actualRules.DestTableName, rules.DestTableName);
			Assert.AreEqual(actualRules.FieldRenames.Count, rules.FieldRenames.Count);
			Assert.AreEqual(actualRules.ExtractedColumns.Count, rules.ExtractedColumns.Count);
			Assert.AreEqual(actualRules.ExtractedAssociations.Count, rules.ExtractedAssociations.Count);

			foreach (var fieldRename in actualRules.FieldRenames) {
				Assert.IsTrue(rules.FieldRenames.ContainsKey(fieldRename.Key));
				Assert.AreEqual(fieldRename.Value, rules.FieldRenames[fieldRename.Key]);
			}

			foreach (var extractedCol in actualRules.ExtractedColumns) {
				Assert.IsTrue(rules.ExtractedColumns.ContainsKey(extractedCol.Key));
				Assert.AreEqual(extractedCol.Value, rules.ExtractedColumns[extractedCol.Key]);
			}

			Assert.IsTrue(rules.ExtractedAssociations.ContainsKey("Responsibility"));
			AssertContainsAll(
				rules.ExtractedAssociations["Responsibility"], 
				actualRules.ExtractedAssociations["Responsibility"]);
		}

		private static void AssertContainsAll(string[] assoc, string[] actualAssoc)
		{
			Assert.IsTrue(Array.TrueForAll(actualAssoc, i => Array.Exists(assoc, j => i == j)));
		}
	}
}
