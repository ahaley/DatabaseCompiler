using System;

namespace Pyrite.DatabaseCompiler.Cli
{
	public class Program
	{
		private static readonly string Quit = "quit";

		static void Main(string[] args)
		{
			var command = "";
			var repository = new CommandRepository();
			while (command != Quit) {
				Console.WriteLine("Commands:");

				Array.ForEach(repository.Commands, s => Console.WriteLine("\t{0}", s));

				Console.Write("Enter Command: ");
				command = Console.ReadLine();

				ICommand commandObject;
				try {
					commandObject = repository.FetchCommand(command);
				}
				catch (Exception) {
					Console.WriteLine("Unknown command {0}", command);
					continue;
				}
				commandObject.PerformCommand();
			}
		}
	}
}
