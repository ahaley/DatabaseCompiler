using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.DatabaseCompiler.RefactorPatterns;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler.Tests
{
	[TestClass]
	public class ExtractAssociationTest
	{
		private readonly Mock<IDatabaseAdapter> src = new Mock<IDatabaseAdapter>();
		private readonly Mock<IDatabaseAdapter> dest = new Mock<IDatabaseAdapter>();
		private readonly Mock<ITableConversionRules> rules = new Mock<ITableConversionRules>();
		private readonly Queue<Dictionary<string, object>> records = new Queue<Dictionary<string, object>>();
		private const string SrcTableName = "SrcTableName";
		private const string DestTableName = "DestTableName";
		private const string FirstExtractedTable = "ExtractedTable";
		private const string SecondExtractedTable = "ExtractedTable2";

		[TestInitialize]
		public void Setup()
		{
			this.rules.Setup(x => x.ExtractedAssociations)
				.Returns(
				new Dictionary<string, string[]> {
					{FirstExtractedTable, new [] {"badField1", "badField2", "badField3", "badField4"}},
					{SecondExtractedTable, new [] {"something1", "something2"}}});

			this.rules.Setup(x => x.SrcTableName).Returns(SrcTableName);
			this.rules.Setup(x => x.DestTableName).Returns(DestTableName);
			this.records.Enqueue(new Dictionary<string, object> {
				{"something1", "somethingvalue1"},
				{"badField1", "value1"},
				{"badField2", "value2"},
				{"something2", "somethingvalue3"},
				{"badField3", "value1"},
				{"badField4", "value2"}});

			this.records.Enqueue(new Dictionary<string, object> {
				{"something1", "somethingvalue2"},
				{"badField1", null},
				{"badField2", "value1"},
				{"something2", "somethingelse2"},
				{"badField3", "value3"},
				{"badField4", "value1"}});

			this.records.Enqueue(null);

			this.src.Setup(x => x.ExtractRecord(SrcTableName))
				.Returns(() => records.Dequeue());
		}

		[TestMethod]
		public void Test_PerformRefactor_ExtractAssociation_Inserts_Into_Extracted_Table()
		{
			// arrange
			var destQueue = new Queue<Dictionary<string, object>>();
			this.dest.Setup(x => x.InsertRecord(FirstExtractedTable, It.IsAny<Dictionary<string, object>>()))
				.Returns(1)
				.Callback((string t, Dictionary<string, object> record) => destQueue.Enqueue(record));
			this.dest.Setup(x => x.InsertRecord(SecondExtractedTable, It.IsAny<Dictionary<string, object>>()))
				.Returns(1)
				.Callback((string t, Dictionary<string, object> record) => destQueue.Enqueue(record));

			var extractAssociation = new ExtractAssociation(this.src.Object, this.dest.Object, this.rules.Object);

			// act
			var result = extractAssociation.PerformRefactor();

			// assert
			this.dest.Verify(x => x.InsertRecord(FirstExtractedTable, new Dictionary<string, object> { { "name", "value1" } }));
			this.dest.Verify(x => x.InsertRecord(FirstExtractedTable, new Dictionary<string, object> { { "name", "value2" } }));
			this.dest.Verify(x => x.InsertRecord(FirstExtractedTable, new Dictionary<string, object> { { "name", "value3" } }));
			this.dest.Verify(x => x.InsertRecord(SecondExtractedTable, new Dictionary<string, object> { { "name", "somethingvalue1" } }));
			this.dest.Verify(x => x.InsertRecord(SecondExtractedTable, new Dictionary<string, object> { { "name", "somethingvalue2" } }));
			this.dest.Verify(x => x.InsertRecord(SecondExtractedTable, new Dictionary<string, object> { { "name", "somethingvalue3" } }));
			this.dest.Verify(x => x.InsertRecord(SecondExtractedTable, new Dictionary<string, object> { { "name", "somethingelse2" } }));
			Assert.AreEqual(7, destQueue.Count);
			Assert.AreEqual(7, result);
		}

		[TestMethod]
		public void Test_PerformRefactor_Creates_ExtractedAssociation()
		{
			// arrange
			var previous = "value1";
			this.dest.Setup(x => x.InsertRecord(FirstExtractedTable, It.IsAny<Dictionary<string, object>>()))
				.Returns(1)
				.Callback((string table, Dictionary<string, object> record) => previous = record["name"].ToString());
			this.dest.Setup(x => x.InsertRecord(SecondExtractedTable, It.IsAny<Dictionary<string, object>>()))
				.Returns(1)
				.Callback((string table, Dictionary<string, object> record) => previous = record["name"].ToString());

			var ids = new Dictionary<string, int> {
				{"value1", 555},
				{"value2", 777},
				{"value3", 888},
				{"somethingvalue1", 123},
				{"somethingvalue2", 321},
				{"somethingvalue3", 322},
				{"somethingelse2", 223}};

			this.dest.Setup(x => x.GetIdLastInsert())
				.Returns(() => ids[previous]);

			var extractAssocation = new ExtractAssociation(this.src.Object, this.dest.Object, this.rules.Object);

			// act
			var result = extractAssocation.PerformRefactor();

			// assert
			var extractedAssocation = extractAssocation.ExtractedAssociation;
			Assert.IsNotNull(extractedAssocation);
			
			var firstValues = new [] {"value1", "value2", "value3"};
			var secondValues = new [] {"somethingvalue1", "somethingvalue2", "somethingvalue3", "somethingelse2"};
		
			foreach (var value in firstValues)
				Assert.AreEqual(ids[value], extractedAssocation.GetIdFromField(FirstExtractedTable, value));

			foreach (var value in secondValues)
				Assert.AreEqual(ids[value], extractedAssocation.GetIdFromField(SecondExtractedTable, value));
		}

		[TestMethod]
		public void Test_GetUniques_Return_Proper_Unique_Values()
		{
			// act
			var result = ExtractAssociation.GetUniques(
				this.records,
				new[] { 
					"badField1",
					"badField2",
					"badField3",
					"badField4"});

			// assert
			Assert.AreEqual(3, result.Length);
			Assert.IsTrue(result.Contains("value1"));
			Assert.IsTrue(result.Contains("value2"));
			Assert.IsTrue(result.Contains("value3"));
		}

		[TestMethod]
		public void Test_GetUniques_Against_Second_Record()
		{
			// act
			var result = ExtractAssociation.GetUniques(
				this.records,
				new[] { "something1", "something2" });

			// assert
			Assert.AreEqual(4, result.Length);
			Assert.IsTrue(result.Contains("somethingvalue1"));
			Assert.IsTrue(result.Contains("somethingvalue2"));
			Assert.IsTrue(result.Contains("somethingvalue3"));
			Assert.IsTrue(result.Contains("somethingelse2"));
		}

		[TestMethod]
		public void Test_PerformRefactor_ExtractAssociation_Inserts_Into_Association_Table()
		{
			// arrange
			const int desttablename_id = 66;
			const int extracttable_id = 55;
			var extractAssocation = new ExtractAssociation(this.src.Object, this.dest.Object, this.rules.Object);
			var record = new Dictionary<string, object> {
				{"badField1", "value1"},
				{"badField2", "value2"}};
			this.dest.Setup(x => x.GetIdLastInsert()).Returns(extracttable_id);

			// act
			var result = extractAssocation.PerformRefactor();
			extractAssocation.InsertAssociationInserts(record, desttablename_id);

			// assert
			this.dest.Verify(x => x.InsertRecord("DestTableNameToExtractedTable",
				new Dictionary<string, object> {
		            {"desttablename_id", desttablename_id},
		            {"extractedtable_id", extracttable_id}}));
		}

		[TestMethod]
		public void GenerateAssociationInserts_Should_Create_Proper_Inserts()
		{
			// arrange
			var record = new Dictionary<string, object> {
				{"badField1", "value2"},
				{"badField2", "value1"}};

			var extractAssociation = new ExtractAssociation(this.src.Object, this.dest.Object, this.rules.Object);

			var result = new List<Dictionary<string, object>>();

			this.dest.Setup(x => x.InsertRecord(It.IsAny<string>(), It.IsAny<Dictionary<string, object>>()))
				.Callback((string s, Dictionary<string, object> r) => result.Add(r));

			extractAssociation.ExtractedAssociation.InsertExtractedField(FirstExtractedTable, 10, "value1");
			extractAssociation.ExtractedAssociation.InsertExtractedField(FirstExtractedTable, 11, "value2");

			// act
			extractAssociation.InsertAssociationInserts(record, 4);

			// assert
			Assert.IsInstanceOfType(result, typeof(List<Dictionary<string, object>>));
			Assert.AreEqual(2, result.Count);
			Assert.AreEqual(4, result[0]["desttablename_id"]);
			Assert.AreEqual(4, result[1]["desttablename_id"]);
			Assert.AreEqual(11, result[0]["extractedtable_id"]);
			Assert.AreEqual(10, result[1]["extractedtable_id"]);
		}
	}
}
