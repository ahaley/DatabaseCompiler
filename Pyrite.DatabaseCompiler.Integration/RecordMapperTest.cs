using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DatabaseCompiler.Integration.Properties;

namespace Pyrite.Db.Converter.Integration
{
	/// <summary>
	/// Summary description for RecordMapperTest
	/// </summary>
	[TestClass]
	public class RecordMapperTest
	{
		protected OleDbConnection src;
		protected SqlConnection dest;

		public RecordMapperTest()
		{
		}

		private TestContext testContextInstance;

		/// <summary>
		///Gets or sets the test context which provides
		///information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get
			{
				return testContextInstance;
			}
			set
			{
				testContextInstance = value;
			}
		}

		[TestInitialize]
		public void TestInitialize()
		{
			var settings = new Settings();
			this.src = new OleDbConnection(settings.GoldConnStr);
			this.src.Open();
			this.dest = new SqlConnection(settings.PyriteConnStr);
			this.dest.Open();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			this.src.Close();
			this.dest.Close();
		}

		[TestMethod]
		public void MapAccountRecords()
		{
			//var mappedFields = new Dictionary<string, string>() {
			//    {"tblContactLoginID", "username"}};

			//var rules = new Mock<ITableConversionRules>();
			//rules.Setup(x => x.MappedFields).Returns(mappedFields);

			//var mapper = RecordMapper.Create("tblContacts", this.dest, rules.Object);
			//var srcFields = new Dictionary<string, object>() {
			//    {"tblContactLoginID", "ahaley"}
			//};
			//string insert = mapper.GenerateInsert(srcFields);
			//Assert.AreEqual("INSERT INTO Account (username) VALUES('ahaley')", insert);
		}

		[TestMethod]
		public void Extract()
		{

		}

	}
}
