/* Copyright (c) 2019 Rick (rick 'at' gibbed 'dot' us)
 * 
 * This software is provided 'as-is', without any express or implied
 * warranty. In no event will the authors be held liable for any damages
 * arising from the use of this software.
 * 
 * Permission is granted to anyone to use this software for any purpose,
 * including commercial applications, and to alter it and redistribute it
 * freely, subject to the following restrictions:
 * 
 * 1. The origin of this software must not be misrepresented; you must not
 *    claim that you wrote the original software. If you use this software
 *    in a product, an acknowledgment in the product documentation would
 *    be appreciated but is not required.
 * 
 * 2. Altered source versions must be plainly marked as such, and must not
 *    be misrepresented as being the original software.
 * 
 * 3. This notice may not be removed or altered from any source
 *    distribution.
 */

using System.Collections.Generic;
using System.IO;
using Gibbed.IO;

namespace Gibbed.Aion.FileFormats
{
    public class BinaryXmlNode
    {
        public string Name { get; private set; }
        public string Value { get; private set; }
        public Dictionary<string, string> Attributes { get; }
        public List<BinaryXmlNode> Children { get; }

        public BinaryXmlNode()
        {
            this.Attributes = new Dictionary<string, string>();
            this.Children = new List<BinaryXmlNode>();
        }

        public void Read(Stream input, BinaryXmlStringTable stringTable)
        {
            this.Name = stringTable[input.ReadValuePackedS32()];
            this.Attributes.Clear();
            this.Children.Clear();
            this.Value = null;

            byte flags = input.ReadValueU8();

            // Value
            if ((flags & 1) == 1)
            {
                this.Value = stringTable[input.ReadValuePackedS32()];
            }

            // Attributes
            if ((flags & 2) == 2)
            {
                int count = input.ReadValuePackedS32();
                for (int i = 0; i < count; i++)
                {
                    var key = stringTable[input.ReadValuePackedS32()];
                    var value = stringTable[input.ReadValuePackedS32()];
                    this.Attributes[key] = value;
                }
            }

            // Children
            if ((flags & 4) == 4)
            {
                int count = input.ReadValuePackedS32();
                for (int i = 0; i < count; i++)
                {
                    var child = new BinaryXmlNode();
                    child.Read(input, stringTable);
                    this.Children.Add(child);
                }
            }
        }
    }
}
