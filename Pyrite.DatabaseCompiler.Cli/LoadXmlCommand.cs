using System;
using System.IO;
using Pyrite.XmlConversionRules;

namespace Pyrite.DatabaseCompiler.Cli
{
	public class LoadXmlCommand : ICommand
	{
		public void PerformCommand()
		{
			Console.Write("Enter filename: ");
			var filename = Console.ReadLine();
			if (!File.Exists(filename)) {
				Console.WriteLine("Could not find {0}", filename);
				return;
			}
			var document = File.ReadAllText(filename);
			var loader = new XmlConversionLoader();
		}
	}
}
