using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.WRLD
{
	public sealed class WRLD_DATASubRecord : SubRecord
	{
		public WRLD_DATAFlags Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Value = (WRLD_DATAFlags)reader.ReadByte();
		}
	}

	[Flags]
	public enum WRLD_DATAFlags
	{
		SmallWorld = 0x01,
		CantFastTravel = 0x02,
		Unknown1 = 0x04,
		Unknown2 = 0x08,
		NoLODWater = 0x10,
		NoLODNoise = 0x20,
		DontAllowNPCFallDamage = 0x40,
		NeedsWaterAdjustment = 0x80
	}
}
