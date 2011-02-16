using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.DatabaseCompiler.RefactorPatterns;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// ExtractColumn is responsible for extracting a column from a database table for the purpose of normalizing that 
	/// column when migrating to another database schema. The column is extracted into a standalone table of the form
	/// {id, name}, where name contains the unique values found from the column of the original table. ExtractColumn
	/// is also responsible with storing the {id, name} tuples in memory since that association is needed when migrating
	/// the original database table's data into the normalized table. The normalized table will contain foreign keys
	/// that reference the extracted column through id.
	/// </summary>
	[TestClass]
	public class ExtractColumnTest
	{
		private readonly Mock<IDatabaseAdapter> src = new Mock<IDatabaseAdapter>();
		private readonly Mock<IDatabaseAdapter> dest = new Mock<IDatabaseAdapter>();
		private readonly Mock<ITableConversionRules> rules = new Mock<ITableConversionRules>();

		[TestInitialize]
		public void Setup()
		{
			this.rules.Setup(x => x.SrcTableName).Returns("source");
			this.rules.Setup(x => x.DestTableName).Returns("destination");
			this.rules.Setup(x => x.ExtractedColumns).Returns(
				new Dictionary<string, string> {{"source_column", "extracted_table"}});

			this.src.Setup(x => x.ExtractColumnDistinct("source", "source_column"))
				.Returns(new object[] {"column_value"});
		}

		[TestMethod]
		public void Test_Correct_Insert_Query_When_Extracting_Column()
		{
			// arrange
			var extractColumn = new ExtractColumn(src.Object, dest.Object, rules.Object);

			// act			
			extractColumn.PerformRefactor();

			// assert
			dest.Verify(x => x.InsertRecord(
				"extracted_table", 
				new Dictionary<string, object> {{"name", "column_value"}}));
		}

		[TestMethod]
		public void Test_Correct_ExtractedColumns_Created()
		{
			// arrange
			var extractColumn = new ExtractColumn(src.Object, dest.Object, rules.Object);
			dest.Setup(x => x.GetIdLastInsert()).Returns(445);

			// act
			extractColumn.PerformRefactor();

			// assert
			var extractedColumns = extractColumn.ForeignKeyResolver;
			Assert.AreEqual(445, extractedColumns.GetForeignKey("extracted_table", "column_value"));
		}

	}
}
