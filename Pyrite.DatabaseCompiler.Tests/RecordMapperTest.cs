using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DatabaseCompiler.RefactorPatterns;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for RecordMapperTest
	/// </summary>
	[TestClass]
	public class RecordMapperTest
	{
		[TestMethod]
		public void Test_MapRecord_Properly_Maps_Record()
		{
			// arrange
			var preMappedRecord = new Dictionary<string, object> { { "tblContactLoginID", "ahaley"} };
			var mapper = new RecordMapper(new AccountConversionRules());

			// act
			var mappedRecord = mapper.MapRecord(preMappedRecord);

			// assert
			Assert.IsTrue(mappedRecord.Keys.Contains("username"));
			Assert.AreEqual("ahaley", mappedRecord["username"]);
		}

		[TestMethod]
		public void Test_MapRecord_Properly_Maps_Record_With_ExtractedColumns()
		{
			// arrange
			var sourceRecord = new Dictionary<string, object> {
				{"tblContactCat", "Developer"},
				{"tblContactLoginID", "ahaley"}};
			//var sourceColumn = "tblContactCat";
			var extractedColumns = new ForeignKeyResolver();
			extractedColumns.InsertID("Title", "Developer", 4);
			var mapper = new RecordMapper(new AccountConversionRules(), extractedColumns);

			// act
			var mappedRecord = mapper.MapRecord(sourceRecord);

			// assert
			Assert.IsTrue(mappedRecord.Keys.Contains("title_id"));
			Assert.AreEqual(4, mappedRecord["title_id"]);
		}

		[TestMethod]
		public void Test_MapRecord_With_Missing_ExtractColumn_Ignores_Record()
		{
			// arrange
			var sourceRecord = new Dictionary<string, object> { { "tblContactCat", "Developer" } };
			var extractedColumns = new ForeignKeyResolver(); // purposefully not loading it with id, field
			var mapper = new RecordMapper(new AccountConversionRules(), extractedColumns);

			// act
			var mappedRecord = mapper.MapRecord(sourceRecord);

			// assert
			Assert.IsFalse(mappedRecord.Keys.Contains("title_id"));

		}

		[TestMethod]
		public void Test_MapRecord_Does_Nothing_With_Null_Rules()
		{
			// arrange
			var record = new Dictionary<string, object> {{"tblContactLoginID", "ahaley"}};
			var mapper = new RecordMapper(null);

			// act
			var mappedRecord = mapper.MapRecord(record);

			// assert
			Assert.IsTrue(mappedRecord.Keys.Contains("tblContactLoginID"));
			Assert.AreEqual("ahaley", mappedRecord["tblContactLoginID"]);
		}

		[TestMethod]
		public void Test_MapRecord_Properly_Maps_Record_With_ExtractedAssocations()
		{
			// arrange
			var sourceRecord = new Dictionary<string, object> {
				{"tblResponsible1", "value1"},
				{"tblResponsible2", "value2"}};
			var mapper = new RecordMapper(new AccountConversionRules());
			
			// act
			var mappedRecord = mapper.MapRecord(sourceRecord);

			// assert


		}

	}
}
