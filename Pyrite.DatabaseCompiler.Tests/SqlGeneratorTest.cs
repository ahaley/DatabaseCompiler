using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DataLayer;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for TableGeneratorTest
	/// </summary>
	[TestClass]
	public class SqlGeneratorTest
	{
		private readonly Mock<DbConnection>	conn = new Mock<DbConnection>();
		private readonly SqlGenerator generator;

		public SqlGeneratorTest()
		{
			generator = new SqlGenerator(conn.Object);
		}

		[TestMethod]
		public void Generate_Insert_With_User_Values()
		{
			// arrange
			var userValues = new Dictionary<string, object> {{"tblContactLoginID", "ahaley"}};

			// act
			var insert = generator.GenerateInsert("tblContacts", userValues);

			// assert
			Assert.AreEqual("INSERT INTO tblContacts (tblContactLoginID) VALUES('ahaley')", insert);
		}

		[TestMethod]
		public void Test_Select_Statement_Generation()
		{
			// arrange
			var schema = new DataTable();
			schema.Columns.Add("COLUMN_NAME");
			schema.Rows.Add(new object[] { "field1" });
			schema.Rows.Add(new object[] { "field2" });
			schema.Rows.Add(new object[] { "field3" });
			conn.Setup(x => x.GetSchema("Columns", new [] { null, null, "testTable", null }))
				.Returns(schema);
			var generator = new SqlGenerator(conn.Object);
			
			// act
			var select = generator.GenerateSelect("testTable");

			// assert
			Assert.AreEqual("SELECT field1, field2, field3 FROM testTable", select);
		}

		[TestMethod]
		public void Generate_Select_With_Where()
		{
			// arrange
			var conn = new Mock<DbConnection>();
			var schema = new DataTable();
			schema.Columns.Add("COLUMN_NAME");
			schema.Rows.Add(new object[] { "field1" });
			schema.Rows.Add(new object[] { "field2" });
			conn.Setup(x => x.GetSchema("Columns", new [] { null, null, "testTable", null }))
				.Returns(schema);
			var generator = new SqlGenerator(conn.Object);

			// act
			var select = generator.GenerateSelect("testTable", "tblContactLoginID = 'ahaley'");
			
			// assert
			Assert.AreEqual(
				"SELECT field1, field2 FROM testTable WHERE tblContactLoginID = 'ahaley'",
				select);
		}

	}
}
