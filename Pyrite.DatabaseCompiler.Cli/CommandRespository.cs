using System;
using System.Collections.Generic;
using System.Linq;

namespace Pyrite.DatabaseCompiler.Cli
{
	public class CommandRepository
	{
		private Dictionary<string, ICommand> commands = new Dictionary<string, ICommand>();

		public CommandRepository()
		{
			this.commands.Add("template", new TemplateCommand());
			this.commands.Add("loadxml", new LoadXmlCommand());
		}

		public string[] Commands { get { return this.commands.Keys.ToArray(); } }

		public ICommand FetchCommand(string commandStr)
		{
			return this.commands[commandStr];
		}
	}
}
