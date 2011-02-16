using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Xml.Linq;
using Pyrite.DataLayer;

namespace Pyrite.XmlConversionRules.Tests
{
	/// <summary>
	/// Summary description for XmlGenerator
	/// </summary>
	[TestClass]
	public class XmlGeneratorTest
	{
		[TestMethod]
		public void Test_XmlGenerator_Constructor()
		{
			// arrange
			var adapter = new Mock<IDatabaseAdapter>();

			// act
			var xmlGenerator = new XmlConversionGenerator(adapter.Object);

			// assert
			Assert.IsNotNull(xmlGenerator);
		}

		[TestMethod]
		public void Test_ConversionGenerator_Generates_Correct_Xml()
		{
			// arrange
			var adapter = new Mock<IDatabaseAdapter>();
			adapter.Setup(x => x.GetSchema("tblContacts"))
				.Returns(new [] { "one", "two", "three" });
			var generator = new XmlConversionGenerator(adapter.Object);

			// act
			var result = generator.GenerateXmlConversionTemplate("tblContacts");

			// assert
			Assert.IsInstanceOfType(result, typeof(ConversionDocument));
			result.Save("test.xml");
		}

		[TestMethod]
		public void Test_XmlConversionLoader_Loads_Correct_Xml()
		{
			// arrange
			var document = XDocument.Parse(Properties.Resources.AssociationConversionRules);

			// act
			var result = XmlConversionLoader.LoadConversionDocument(document);

			// assert
			Assert.IsInstanceOfType(result, typeof(ITableConversionRules));
		}

	}
}
