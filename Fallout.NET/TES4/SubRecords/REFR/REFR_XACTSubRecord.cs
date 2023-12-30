using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XACTSubRecord : SubRecord
	{
		public REFR_XACTFlags Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Value = (REFR_XACTFlags)reader.ReadUInt32();
		}
	}

	[Flags]
	public enum REFR_XACTFlags
	{
		UseDefault = 0x01,
		Activate = 0x02,
		Open = 0x04,
		OpenByDefault = 0x08
	}
}
