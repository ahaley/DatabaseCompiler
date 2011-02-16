using System;
using System.Data;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DataLayer;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for DataExtractorTest
	/// </summary>
	[TestClass]
	public class DataExtractorTest
	{
		private Mock<IDataReader> reader;
		private static readonly string[] fields = new [] { "field1", "field2", "field3" };

		[TestInitialize]
		public void Setup()
		{
			reader = new Mock<IDataReader>();
		}

		[TestMethod]
		public void Test_ExtractRecord_With_Null_Value()
		{
			// arrange
			reader.Setup(x => x.Read()).Returns(true);
			reader.Setup(x => x["field1"]).Returns("what1");
			reader.Setup(x => x["field2"]).Returns(DBNull.Value);
			reader.Setup(x => x["field3"]).Returns("what3");

			// act
			var record = DatabaseAdapter.ExtractRecord(reader.Object, fields);

			// assert
			Assert.AreEqual(2, record.Count);
			Assert.AreEqual("what1", record["field1"] as string);
			Assert.AreEqual("what3", record["field3"] as string);
		}

		[TestMethod]
		public void Test_ExtractRecord_With_Read_False()
		{
			// assert
			reader.Setup(x => x.Read()).Returns(false);

			// act
			var record = DatabaseAdapter.ExtractRecord(reader.Object, fields);

			// assert
			Assert.IsNull(record);

		}
	}
}
