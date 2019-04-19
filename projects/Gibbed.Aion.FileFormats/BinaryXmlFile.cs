using System;
using System.Collections.Generic;
using System.IO;
using Gibbed.Helpers;

namespace Gibbed.Aion.FileFormats
{
	public static class BinaryXmlFileHelpers
	{
		public static Int32 ReadValuePackedS32(this Stream stream)
		{
			byte current = stream.ReadValueU8();
			int value = 0;
			int shift = 0;

			while (current >= 0x80)
			{
				value |= (current & 0x7F) << shift;
				shift += 7;
				current = stream.ReadValueU8();
			}

			return value | (current << shift);
		}

		public static string ReadTable(this byte[] data, int offset)
		{
			if (offset == 0)
			{
				return "";
			}

			return data.ToStringUTF16Z(2 * offset);
		}
	}

	public class BinaryXmlStringTable
	{
		protected byte[] Data;

		public void Read(Stream input)
		{
			int size = input.ReadValuePackedS32();
			this.Data = new byte[size];
			input.Read(this.Data, 0, size);
		}

		public string this[int index]
		{
			get
			{
				if (index == 0)
				{
					return "";
				}

				return this.Data.ToStringUTF16Z(index * 2);
			}
		}
	}

	public class BinaryXmlNode
	{
		public string Name;
		public string Value;
		public Dictionary<string, string> Attributes;
		public List<BinaryXmlNode> Children;

		public void Read(Stream input, BinaryXmlStringTable table)
		{
			this.Name = table[input.ReadValuePackedS32()];
			this.Attributes = new Dictionary<string, string>();
			this.Children = new List<BinaryXmlNode>();
			this.Value = null;

			byte flags = input.ReadValueU8();

			// Value
			if ((flags & 1) == 1)
			{
				this.Value = table[input.ReadValuePackedS32()];
			}

			// Attributes
			if ((flags & 2) == 2)
			{
				int count = input.ReadValuePackedS32();
				for (int i = 0; i < count; i++)
				{
					string key = table[input.ReadValuePackedS32()];
					string value = table[input.ReadValuePackedS32()];
					this.Attributes[key] = value;
				}
			}

			// Children
			if ((flags & 4) == 4)
			{
				int count = input.ReadValuePackedS32();
				for (int i = 0; i < count; i++)
				{
					BinaryXmlNode child = new BinaryXmlNode();
					child.Read(input, table);
					this.Children.Add(child);
				}
			}
		}
	}

	public class BinaryXmlFile
	{
		public BinaryXmlNode Root;

		public void Read(Stream input)
		{
			if (input.ReadValueU8() != 0x80)
			{
				throw new InvalidDataException("not a binary XML file");
			}

			BinaryXmlStringTable table = new BinaryXmlStringTable();
			table.Read(input);

			this.Root = new BinaryXmlNode();
			this.Root.Read(input, table);
		}
	}
}
