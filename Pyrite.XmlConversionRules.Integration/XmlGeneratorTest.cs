using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Pyrite.XmlConversionRules.Integration
{
	/// <summary>
	/// Summary description for UnitTest1
	/// </summary>
	[TestClass]
	public class XmlGeneratorTest
	{
		private OledbConnection conn;

		[TestInitialize]
		public void Setup()
		{
			this.conn = new OleDbConnection(new Settings().GoldConnStr);
		}

		[TestMethod]
		public void TestMethod1()
		{
			//
			// TODO: Add test logic	here
			//
		}
	}
}
