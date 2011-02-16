using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DataLayer;

namespace Pyrite.DatabaseCompiler.Tests
{
	/// <summary>
	/// Summary description for DbTypeResolverTests
	/// </summary>
	[TestClass]
	public class TypeResolverTests
	{
		[TestMethod]
		public void Test_Format_String_Value()
		{
			// act
			var result = TypeResolver.FormatValue("test");

			// assert
			Assert.AreEqual("'test'", result);
		}

		[TestMethod]
		public void Test_Format_Int_Value()
		{
			// act
			var result = TypeResolver.FormatValue(5);

			// assert
			Assert.AreEqual("5", result);
		}

		[TestMethod]
		public void Test_Format_Composite_Values()
		{
			// arrange
			var values = new object[] { "one", "two", 3};

			// act
			var result = TypeResolver.FormatValues(values);

			// assert
			Assert.AreEqual("'one'", result[0]);
			Assert.AreEqual("'two'", result[1]);
			Assert.AreEqual("3", result[2]);
		}

		[TestMethod]
		public void Single_Quote_Escaped()
		{
			// arrange
			var values = new object[] { "O'Brien", "Lok'tar" };
			
			// act
			var result = TypeResolver.FormatValues(values);

			// assert
			Assert.AreEqual("'O''Brien'", result[0]);
			Assert.AreEqual("'Lok''tar'", result[1]);
		}
	}
}
