using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.WRLD
{
	public sealed class WRLD_PNAMSubRecord : SubRecord
	{
		public WRLD_PNAMFlags Flags;
		public byte Unknown;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Flags = (WRLD_PNAMFlags)reader.ReadByte();
			Unknown = reader.ReadByte();
		}
	}

	[Flags]
	public enum WRLD_PNAMFlags
	{
		UseLandData = 0x01,
		UseLODData = 0x02,
		UseMapData = 0x04,
		UseWaterData = 0x08,
		UseClimateData = 0x10,
		UseImageSpaceData = 0x20,
		Unknown = 0x40,
		NeedsWaterAdjustment = 0x80
	}
}
