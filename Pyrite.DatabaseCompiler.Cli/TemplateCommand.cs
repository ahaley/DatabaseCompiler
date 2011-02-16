using System;
using Pyrite.XmlConversionRules;
using System.Data.OleDb;
using Pyrite.DatabaseCompiler.Cli.Properties;
using System.Reflection;
using System.IO;
using Pyrite.DataLayer;

namespace Pyrite.DatabaseCompiler.Cli
{
	public class TemplateCommand : ICommand
	{
		public void PerformCommand()
		{
			Console.Write("Enter table to create template from: ");
			var table = Console.ReadLine();

			var conn = new OleDbConnection(new Settings().GoldConnStr);
			conn.Open();

			var generator = new XmlConversionGenerator(
				new DatabaseAdapter(conn));

			var document = generator.GenerateXmlConversionTemplate(table);

			var assembly = Assembly.GetExecutingAssembly();
			var dir = Path.GetDirectoryName(assembly.Location);
			var file = Path.Combine(dir, String.Format("{0}.xml", table));

			document.Save(file);
		}
	}
}
