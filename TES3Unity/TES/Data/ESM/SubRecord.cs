using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Fallout.NET;

namespace TES3Unity.ESM
{
	public struct SubRecordHeader
	{
		public string name; // 4 bytes
		public uint dataSize;

		public void Deserialize(ITesReader reader)
		{
			name = reader.ReadASCIIString(4);
			dataSize = reader.ReadUInt32();
		}
	}

	public abstract class SubRecord
	{
		public SubRecordHeader header;
		public abstract void DeserializeData(ITesReader reader, uint dataSize);
	}
}
