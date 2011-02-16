using System;
using System.Xml.Linq;

namespace Pyrite.XmlConversionRules
{
	public class ConversionDocument
	{
		private XDocument doc;

		public ConversionDocument(XDocument doc)
		{
			this.doc = doc;
		}

		public void Save(string filename)
		{
			this.doc.Save(filename);
		}

	}
}
