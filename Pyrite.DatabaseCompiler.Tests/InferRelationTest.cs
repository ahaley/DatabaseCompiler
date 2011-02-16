using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;
using System.Collections.Generic;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for InferRelation
	/// </summary>
	[TestClass]
	public class InferRelationTest
	{
		private Mock<IDatabaseAdapter> src;
		private Mock<IDatabaseAdapter> dest;
		private Mock<ITableConversionRules> rules;

		[TestInitialize]
		public void Setup()
		{
			src = new Mock<IDatabaseAdapter>();
			dest = new Mock<IDatabaseAdapter>();
			rules = new Mock<ITableConversionRules>();

			var inferRelations = new Dictionary<string, string[]> {
				{"Account", new [] {"firstname", "lastname"}}};

			rules.Setup(x => x.InferRelations).Returns(inferRelations);

			var inputQueue = new Queue<Dictionary<string, object>>();
			inputQueue.Enqueue(new Dictionary<string, object> {
				{"so_num", "1"},
				{"firstname", "tony"},
				{"lastname", "haley"}});
			inputQueue.Enqueue(new Dictionary<string, object> {
				{"so_num", "2"},
				{"firstname", "john"},
				{"lastname", "doe"}});
			inputQueue.Enqueue(null);

			src.Setup(x => x.ExtractRecord("oldSO"))
				.Returns(() => inputQueue.Dequeue());
		}

		[TestMethod]
		public void Test_Relation_Is_Inferred_When_Record_Exists()
		{
			//var inferRelation = new InferRelation(src, dest, rules);
			//var result = inferRelation.PerformRefactor();

		}
	}
}
