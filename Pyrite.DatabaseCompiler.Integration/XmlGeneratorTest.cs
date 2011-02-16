using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Data.OleDb;
using Pyrite.DatabaseCompiler.Integration.Properties;
using Pyrite.XmlConversionRules;
using Pyrite.DataLayer;

namespace Pyrite.DatabaseCompiler.Integration
{
	/// <summary>
	/// Summary description for XmlGeneratorTest
	/// </summary>
	[TestClass]
	public class XmlGeneratorTest
	{
		private OleDbConnection conn;

		[TestInitialize]
		public void Setup()
		{
			this.conn = new OleDbConnection(new Settings().GoldConnStr);
			this.conn.Open();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			this.conn.Close();
		}

		[TestMethod]
		public void Test_Generate_Conversion_Xml_Against_Live_Database()
		{
			var adapter = new DatabaseAdapter(this.conn);
			var generator = new XmlConversionGenerator(adapter);

			var result = generator.GenerateXmlConversionTemplate("tblContacts");

			string path = @"C:\Users\ahaley\Documents\Visual Studio 2008\Projects\Pyrite\Pyrite.XmlConversionRules.Tests\xml\test.xml";
			result.Save(path);

		}
	}
}
