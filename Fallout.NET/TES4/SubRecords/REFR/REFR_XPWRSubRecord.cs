using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XPWRSubRecord : SubRecord
	{
		public uint Reference;
		public REFR_XPWRFlags Flags;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Reference = reader.ReadUInt32();
			Flags = (REFR_XPWRFlags)reader.ReadUInt32();
		}
	}

	[Flags]
	public enum REFR_XPWRFlags
	{
		Reflection = 0x01,
		Refraction = 0x02
	}
}
