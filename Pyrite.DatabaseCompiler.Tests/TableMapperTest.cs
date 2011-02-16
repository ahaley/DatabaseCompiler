using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for TableMapperTest
	/// </summary>
	[TestClass]
	public class TableMapperTest
	{
		private Mock<IDatabaseAdapter> src;
		private Mock<IDatabaseAdapter> dest;
		private Mock<ITableConversionRules> rules = new Mock<ITableConversionRules>();
		private Mock<IRecordMapper> recordMapper = new Mock<IRecordMapper>();
		private const string SrcTableName = "SrcTableName";
		private const string DestTableName = "DestTableName";
		private readonly Dictionary<string, object> record1 = new Dictionary<string, object> { { "field1", "value1" } };
		private readonly Dictionary<string, object> record2 = new Dictionary<string, object> { { "field2", "value2" } };
		private Queue<Dictionary<string, object>> table;

		[TestInitialize]
		public void Setup()
		{
			this.table = new Queue<Dictionary<string, object>>();
			this.table.Enqueue(record1);
			this.table.Enqueue(record2);
			this.table.Enqueue(null);

			this.src = new Mock<IDatabaseAdapter>();
			this.src.Setup(x => x.ExtractRecord(SrcTableName))
				.Returns(() => this.table.Dequeue());

			this.dest = new Mock<IDatabaseAdapter>();
			this.dest.Setup(x => x.InsertRecord(DestTableName, record1)).Returns(1);
			this.dest.Setup(x => x.InsertRecord(DestTableName, record2)).Returns(1);

			this.rules = new Mock<ITableConversionRules>();
			this.rules.Setup(x => x.SrcTableName).Returns(SrcTableName);
			this.rules.Setup(x => x.DestTableName).Returns(DestTableName);

			this.recordMapper = new Mock<IRecordMapper>();
			this.recordMapper.Setup(x => x.MapRecord(record1)).Returns(record1);
			this.recordMapper.Setup(x => x.MapRecord(record2)).Returns(record2);
		}

		[TestMethod]
		public void Test_InsertMappedFields_With_RecordMapper()
		{
			// arrange
			var tableMapper = new TableMapper(this.src.Object, this.dest.Object, this.rules.Object);
			
			// act
			var result = tableMapper.InsertMappedFields(this.recordMapper.Object);

			// assert
			Assert.AreEqual(2, result);

		}
	}
}
