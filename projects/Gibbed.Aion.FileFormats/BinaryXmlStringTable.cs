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

using System;
using System.IO;
using System.Text;
using Gibbed.IO;

namespace Gibbed.Aion.FileFormats
{
    public class BinaryXmlStringTable
    {
        private byte[] _Buffer;

        public void Read(Stream input)
        {
            int size = input.ReadValuePackedS32();
            this._Buffer = input.ReadBytes(size);
        }

        private string GetString(int index)
        {
            var start = index * 2;
            var end = this._Buffer.Length;
            if (start < 0 || start >= end)
            {
                throw new ArgumentOutOfRangeException("index");
            }
            for (int i = start; i + 2 <= end; i += 2)
            {
                if (this._Buffer[i + 0] == 0 &&
                    this._Buffer[i + 1] == 0)
                {
                    end = i;
                    break;
                }
            }
            return Encoding.Unicode.GetString(this._Buffer, start, end - start);
        }

        public string this[int index]
        {
            get
            {
                return index == 0 ? string.Empty : this.GetString(index);
            }
        }
    }
}
