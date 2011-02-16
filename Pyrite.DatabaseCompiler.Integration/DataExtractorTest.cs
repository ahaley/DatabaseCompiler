using System;
using System.Data.OleDb;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DatabaseCompiler.Integration.Properties;
using Pyrite.DataLayer;

namespace Pyrite.Db.Converter.Integration
{
	/// <summary>
	/// Summary description for DataExtractorTest
	/// </summary>
	[TestClass]
	public class DataExtractorTest
	{
		private OleDbConnection conn;

		[TestInitialize]
		public void TestInitialize()
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
		public void ExtractFields()
		{
			var adapter = new DatabaseAdapter(this.conn);
			var fields = adapter.ExtractColumnDistinct("tblContacts", "tblContactCat");
			Array.ForEach(fields, field => Console.WriteLine("{0}", field));
		}

        /*
		[TestMethod]
		public void ExtractRecord()
		{
			string[] fields = new string[] { "field1", "field2", "field3" };
			var reader = new Mock<IDataReader>();
			reader.Setup(x => x["field1"]).Returns("what1");
			reader.Setup(x => x["field2"]).Returns(DBNull.Value);
			reader.Setup(x => x["field3"]).Returns("what3");

			var extractor = new DataExtractor(fields);
			Dictionary<string, object> record = extractor.ExtractRecord(reader.Object);
			Assert.AreEqual(2, record.Count);
			Assert.AreEqual("what1", record["field1"] as string);
			Assert.AreEqual("what3", record["field3"] as string);
		}*/
	}
}
