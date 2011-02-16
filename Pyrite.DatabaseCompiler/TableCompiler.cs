using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Xml.Linq;
using Pyrite.DataLayer;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler
{
	public class TableCompiler
	{
		private readonly OleDbConnection src;
		private readonly SqlConnection dest;

		public TableCompiler(OleDbConnection src, SqlConnection dest)
		{
			this.src = src;
			this.dest = dest;
		}

		public void Compile(string[] tableNames)
		{
			foreach (var tableName in tableNames) {
				var mapper = TableMapperFactory.Create(
					new DatabaseAdapter(src), new DatabaseAdapter(dest), tableName);
				mapper.MapTable();
			}
		}

		public void Compile(string path) 
		{
			var info = new DirectoryInfo(path);
			foreach (var fileInfo in info.GetFiles()) {
				var filename = fileInfo.Name;
				var doc = XDocument.Load(filename);
				var rules = XmlConversionLoader.LoadConversionDocument(doc);
				(new TableMapper(
					new DatabaseAdapter(src), 
					new DatabaseAdapter(dest), rules)).MapTable(); 
			}

		}
	}
}
