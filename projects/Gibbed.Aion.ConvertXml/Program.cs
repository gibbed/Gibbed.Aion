using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows.Forms;
using System.Xml;
using Gibbed.Aion.FileFormats;

namespace Gibbed.Aion.ConvertXml
{
	class Program
	{
		private static void WriteNode(XmlWriter writer, BinaryXmlNode node)
		{
			writer.WriteStartElement(node.Name);

			foreach (KeyValuePair<string, string> attribute in node.Attributes)
			{
				writer.WriteAttributeString(attribute.Key, attribute.Value);
			}

			foreach (BinaryXmlNode child in node.Children)
			{
				WriteNode(writer, child);
			}

			if (node.Value != null)
			{
				writer.WriteValue(node.Value);
			}

			writer.WriteEndElement();
		}

		public static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("{0} [input_binary.xml] [output.xml]", Path.GetFileName(Application.ExecutablePath));
				return;
			}

			Stream input = File.OpenRead(args[0]);
			BinaryXmlFile bx = new BinaryXmlFile();
			bx.Read(input);
			input.Close();

			XmlTextWriter writer = new XmlTextWriter(args[1], Encoding.Unicode);
			writer.Formatting = Formatting.Indented;

			writer.WriteStartDocument();
			WriteNode(writer, bx.Root);
			writer.WriteEndDocument();
			writer.Flush();
			writer.Close();
		}
	}
}
