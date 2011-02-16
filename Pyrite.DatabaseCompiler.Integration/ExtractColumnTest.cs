using System;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DatabaseCompiler.Integration.Properties;
using Pyrite.DatabaseCompiler.RefactorPatterns;
using Pyrite.XmlConversionRules;

namespace Pyrite.Db.Converter.Integration
{
	/// <summary>
	/// Summary description for ExtractColumnTest
	/// </summary>
	[TestClass]
	public class ExtractColumnTest
	{
		private IDbConnection src;
		private IDbConnection dest;

		[TestInitialize]
		public void Setup()
		{
			var settings = new Settings();
			this.src = new OleDbConnection(settings.GoldConnStr);
			this.dest = new SqlConnection(settings.PyriteConnStr);
			this.src.Open();
			this.dest.Open();

			var command = this.dest.CreateCommand();
			var tables = new string[] { "AccountToResponsibility", "Account", "Department", "Office", "Title", "Responsibility" };

			foreach (string table in tables) {
				command.CommandText = "DELETE FROM " + table;
				command.ExecuteNonQuery();
			}
		}

		[TestCleanup]
		public void TestCleanup()
		{
			this.src.Close();
			this.dest.Close();
		}

		[TestMethod]
		public void TestInsertExtractedColumns()
		{
			var rules = new AccountConversionRules();
			var extractColumn = new ExtractColumn(this.src, this.dest, rules);
			var count = extractColumn.PerformRefactor();
		}

		//[TestMethod]
		//public void TestDistinct()
		//{
		//    var command = this.src.CreateCommand();
		//    command.CommandText = "SELECT DISTINCT tblContactOffice FROM tblContacts";
		//    var reader = command.ExecuteReader();

		//    while (reader.Read()) {
		//        Console.WriteLine(reader["tblContactOffice"].ToString());
		//    }
		//}

	}
}
