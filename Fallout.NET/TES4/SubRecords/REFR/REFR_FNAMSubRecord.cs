using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_FNAMSubRecord : SubRecord
	{
		public REFR_FNAMFlags Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Value = (REFR_FNAMFlags)reader.ReadByte();
		}
	}

	[Flags]
	public enum REFR_FNAMFlags
	{
		Visible = 0x01,
		CanTravelTo = 0x02,
		ShowAllHidden = 0x04
	}
}
