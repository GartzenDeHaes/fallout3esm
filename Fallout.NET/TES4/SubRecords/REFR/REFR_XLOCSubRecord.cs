using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XLOCSubRecord : SubRecord
	{
		public byte Level;
		public byte[] Unused;
		public uint Key;
		public byte Flags;
		public byte[] Unknown;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Level = reader.ReadByte();
			Unused = reader.ReadBytes(3);
			Key = reader.ReadUInt32();
			if (Size == 12)
			{
				Unknown = reader.ReadBytes(4);
			}
			else if (Size == 16)
			{
				Unknown = reader.ReadBytes(8);
			}
			else if (Size == 20)
			{
				Unknown = reader.ReadBytes(12);
			}
		}
	}
}
