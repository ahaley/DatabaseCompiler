using System;
using System.Data;
using System.Data.Common;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DataLayer;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for FieldDescriptorTest
	/// </summary>
	[TestClass]
	public class TableDescriptorTest
	{
		private DataTable schema;

		[TestInitialize]
		public void TestInitialize()
		{
			schema = new DataTable();
			schema.Columns.Add("COLUMN_NAME");
			schema.Rows.Add(new object[] { "field1" });
			schema.Rows.Add(new object[] { "field2" });
			schema.Rows.Add(new object[] { "field3" });
		}

		[TestMethod]
		public void Get_Fields()
		{
			// arrange
			var conn = new Mock<DbConnection>();
			var restrictions = new [] { null, null, "tableName", null };
			conn.Setup(x => x.GetSchema("Columns", restrictions))
				.Returns(schema);
			var descriptor = new TableDescriptor(conn.Object);
			
			// act
			var fields = descriptor.GetFields("tableName");

			// assert
			AssertCorrectSchemaFields(fields);
		}

		private static void AssertCorrectSchemaFields(string[] fields)
		{
			if (fields == null) throw new ArgumentNullException("fields");
			Assert.AreEqual(3, fields.Length);
			Assert.AreEqual("field1", fields[0]);
			Assert.AreEqual("field2", fields[1]);
			Assert.AreEqual("field3", fields[2]);
		}

	}
}
