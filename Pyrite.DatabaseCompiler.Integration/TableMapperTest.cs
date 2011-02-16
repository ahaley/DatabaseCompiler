using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Pyrite.DatabaseCompiler;
using Pyrite.DatabaseCompiler.Integration.Properties;
using Pyrite.DataLayer;
using System.Xml.Linq;
using Pyrite.XmlConversionRules;
using System;

namespace Pyrite.Db.Converter.Integration
{
	[TestClass]
	public class TableMapperTest
	{
		private IDbConnection src;
		private IDbConnection dest;

		[TestInitialize]
		public void Setup()
		{
			src = new OleDbConnection(new Settings().GoldConnStr);
			src.Open();
			dest = new SqlConnection(new Settings().PyriteConnStr);
			dest.Open();

			var command = dest.CreateCommand();
			var tables = new string[] { "AccountToResponsibility", "Responsibility", "Account", "Department", "Office", "Title" };
			Array.ForEach(tables, table => {
				command.CommandText = String.Format("DELETE FROM {0}", table);
				command.ExecuteNonQuery();
			});

			command.CommandText = "DELETE FROM Responsibility";
			command.ExecuteNonQuery();
		}

		[TestCleanup]
		public void TestCleanup()
		{
			src.Close();
			dest.Close();
		}

		[TestMethod]
		public void TestFactoryMethod()
		{
			var tableMapper = TableMapperFactory.Create(
				new DatabaseAdapter(src),
				new DatabaseAdapter(dest),
				"tblContacts");
			Assert.IsInstanceOfType(tableMapper, typeof(TableMapper));
		}

		[TestMethod]
		public void Test_InsertMappedFields_Against_Live_DB()
		{
			var tableMapper = TableMapperFactory.Create(
				new DatabaseAdapter(src),
				new DatabaseAdapter(dest),
				"tblContacts");
			tableMapper.InsertMappedFields();
		}

		[TestMethod]
		public void Test_MapTable_Against_tblContacts()
		{
			var tableMapper = TableMapperFactory.Create(
				new DatabaseAdapter(src),
				new DatabaseAdapter(dest),
				"tblContacts");
			tableMapper.MapTable();
		}

		[TestMethod]
		public void Test_MapTable_Against_Xml()
		{
			var doc = XDocument.Parse(Resources.AssociationConversionRules);
			
			var rules = XmlConversionLoader.LoadConversionDocument(doc);
			var tableMapper = new TableMapper(
				new DatabaseAdapter(src),
				new DatabaseAdapter(dest),
				rules);
			tableMapper.MapTable();
		}

	}
}
