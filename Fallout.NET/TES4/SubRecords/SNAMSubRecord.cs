using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class SNAMSubRecord : SubRecord
	{
		public uint Faction;
		public byte Rank;
		public byte[] Unused;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Faction = reader.ReadUInt32();
			Rank = reader.ReadByte();
			Unused = reader.ReadBytes(3);
		}
	}
}
