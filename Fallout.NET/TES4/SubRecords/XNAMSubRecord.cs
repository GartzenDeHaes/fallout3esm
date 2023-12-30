using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class XNAMSubRecord : SubRecord
	{
		public uint FactionFormId;
		public FactionRelation Flags;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			FactionFormId = reader.ReadUInt32();

			var mod = reader.ReadInt32();

			Flags = (FactionRelation)reader.ReadInt32();
		}
	}

	[Flags]
	public enum FactionRelation
	{
		Neutral = 0,
		Enemy = 1,
		Ally = 2,
		Friend = 3,
	}
}
