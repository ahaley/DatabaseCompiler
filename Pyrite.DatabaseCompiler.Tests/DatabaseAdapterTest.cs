using System.Collections.Generic;
using System.Data;
using Moq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Pyrite.DataLayer;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for DatabaseAdapterTest
	/// </summary>
	[TestClass]
	public class DatabaseAdapterTest
	{
		private readonly Mock<IDbConnection> conn = new Mock<IDbConnection>();
		private readonly Mock<IDbCommand> command = new Mock<IDbCommand>();
		private readonly Mock<IDataReader> reader = new Mock<IDataReader>();
		private readonly Mock<ISqlGenerator> sqlGenerator = new Mock<ISqlGenerator>();
		private const string TableName = "some_table";
		private const string SelectQuery = "SELECT * FROM some_table";
		private static readonly string[] TableDefinition = new[] { "field1", "field2", "field3" };
		private static readonly string[,] TableData = new[,] {
			{"col1val1", "col2val1", "col3val1"},
			{"col1val2", "col2val2", "col3val2"},
			{"col1val3", "col2val3", "col3val3"},
			{"col1val4", "col2val4", "col3val4"}};

		[TestInitialize]
		public void Setup()
		{
			this.conn.Setup(x => x.CreateCommand()).Returns(this.command.Object);
			this.command.Setup(x => x.ExecuteReader()).Returns(this.reader.Object);
			this.sqlGenerator.Setup(x => x.GenerateSelect(TableName))
				.Returns(SelectQuery);
			this.sqlGenerator.Setup(x => x.GetFields(TableName))
				.Returns(TableDefinition);

			for (var i = 0; i < TableDefinition.Length; i++)
				this.reader.Setup(x => x[TableDefinition[i]]).Returns(TableData[0, i]);
		}

		[TestMethod]
		public void Test_ExtractRecord_With_Sql_Query()
		{
			// arrange 
			var adapter = new DatabaseAdapter(this.conn.Object, this.sqlGenerator.Object);
			this.reader.Setup(x => x.Read()).Returns(true);

			// act
			var record = adapter.ExtractRecord(TableName);

			// assert
			Assert_Record_Is_Good(record);
		}

		[TestMethod]
		public void Test_ExtractRecord_Called_Multiple_Times()
		{
			// arrange
			int j = -1;

			this.reader.Setup(x => x["field1"])
				.Returns(() => TableData[j, 0]);
			this.reader.Setup(x => x["field2"])
				.Returns(() => TableData[j, 1]);
			this.reader.Setup(x => x["field3"])
				.Returns(() => TableData[j, 2]);

			this.reader.Setup(x => x.Read())
				.Returns(true)
				.Callback(() => j++);

			var adapter = new DatabaseAdapter(this.conn.Object, this.sqlGenerator.Object);


			// act
			var record1 = adapter.ExtractRecord(TableName);
			var record2 = adapter.ExtractRecord(TableName);

			// assert
			Assert.AreEqual("col2val1", record1["field2"]);
			Assert.AreEqual("col2val2", record2["field2"]);
		}

		[TestMethod]
		public void Calling_ExtractRecord_After_Null_Resets_Reader()
		{
			// arrange
			int j = -1;

			this.reader.Setup(x => x["field2"])
				.Returns(() => j < 4 ? TableData[j, 1] : null);

			this.reader.Setup(x => x.Read())
				.Returns(() => j < 3)
				.Callback(() => j++);

			this.command.Setup(x => x.ExecuteReader())
				.Returns(this.reader.Object)
				.Callback(() => j = -1);

			var adapter = new DatabaseAdapter(this.conn.Object, this.sqlGenerator.Object);

			// act
			var record1 = adapter.ExtractRecord(TableName);
			var record2 = adapter.ExtractRecord(TableName);
			var record3 = adapter.ExtractRecord(TableName);
			var record4 = adapter.ExtractRecord(TableName);
			var record5 = adapter.ExtractRecord(TableName);
			var record6 = adapter.ExtractRecord(TableName);

			// assert
			Assert.AreEqual("col2val1", record1["field2"]);
			Assert.AreEqual("col2val2", record2["field2"]);
			Assert.AreEqual("col2val3", record3["field2"]);
			Assert.AreEqual("col2val4", record4["field2"]);
			Assert.IsNull(record5);
			Assert.AreEqual("col2val1", record6["field2"]);


		}


		[TestMethod]
		public void Test_ExtractColumn_Works()
		{
			// arrange
			var adapter = new DatabaseAdapter(this.conn.Object, this.sqlGenerator.Object);
			var callRead = 0;
			this.reader.Setup(x => x.Read())
				.Returns(() => callRead < 3)
				.Callback(() => callRead++);
			var callArray = 0;
			this.reader.Setup(x => x["field2"])
				.Returns(() => TableData[callArray, 1])
				.Callback(() => callArray++);

			// act
			var columnData = adapter.ExtractColumnDistinct("some_table", "field2");

			// assert
			Assert.AreEqual(3, columnData.Length);
			Assert.AreEqual("col2val1", columnData[0]);
			Assert.AreEqual("col2val2", columnData[1]);
			Assert.AreEqual("col2val3", columnData[2]);
		}

		[TestMethod]
		public void Test_ExtractMultipleColumns_Work()
		{
			// arrange
			var adapter = new DatabaseAdapter(this.conn.Object, this.sqlGenerator.Object);
			var dbInput = new Queue<object>(new []
				{"col2val1", "col2val2", "col2val3", "col3val1", "col3val2", "col3val3"});
			this.reader.Setup(x => x.Read()).Returns(() => dbInput.Count > 0);
			this.reader.Setup(x => x[0]).Returns(() => dbInput.Dequeue());
				
			// act
			var multiColumnData = adapter.ExtractMultiColumnDistinct(TableName, new [] {"field2", "field3"});

			// assert
			Assert.AreEqual(6, multiColumnData.Length);
		}

		[TestMethod]
		public void Test_Insert_Record_Dictionary()
		{
			// arrange
			var adapter = new DatabaseAdapter(this.conn.Object, this.sqlGenerator.Object);
			var record = new Dictionary<string, object> {
				{"field1", "value1"},
				{"field2", "value2"}};

			this.command.Setup(x => x.ExecuteNonQuery()).Returns(1);
			const string query = "INSERT INTO some_table (field1, field2) VALUES('value1', 'value2')";
			this.sqlGenerator.Setup(x => x.GenerateInsert(TableName, record))
				.Returns(query);
			
			// act
			var result = adapter.InsertRecord("some_table", record);
            
			// assert
			command.VerifySet(x => x.CommandText = query);
			command.Verify(x => x.ExecuteNonQuery());
			Assert.AreEqual(1, result);
		}

		private static void Assert_Record_Is_Good(IDictionary<string, object> record)
		{
			Assert.AreEqual(3, record.Count);
			for (var i = 0; i < TableDefinition.Length; i++) {
				Assert.IsTrue(record.ContainsKey(TableDefinition[i]));
				Assert.AreEqual(TableData[0, i], record[TableDefinition[i]]);
			}
		}

	}
}
