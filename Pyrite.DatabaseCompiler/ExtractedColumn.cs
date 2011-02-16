using System.Collections.Generic;
using System.Linq;

namespace Pyrite.DatabaseCompiler
{
	public class ExtractedColumn
	{
		private readonly Dictionary<string, int> _fieldToId = new Dictionary<string, int>();

		/// <summary>
		/// 
		/// </summary>
		/// <param name="name">The name of the source column.</param>
		public ExtractedColumn(string name)
		{
			Name = name;
		}

		/// <summary>
		/// The name of the source column.
		/// </summary>
		public string Name { get; private set; }

		public int GetId(string field)
		{
			if (_fieldToId.Keys.Contains(field))
				return _fieldToId[field];
			return -1;
		}

		public void InsertField(int id, string field)
		{
			_fieldToId.Add(field, id);
		}
	}
}
