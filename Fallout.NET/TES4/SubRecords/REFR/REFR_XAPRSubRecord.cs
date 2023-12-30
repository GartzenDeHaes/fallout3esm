using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XAPRSubRecord : SubRecord
	{
		public uint Reference;
		public float Delay;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Reference = reader.ReadUInt32();
			Delay = reader.ReadSingle();
		}
	}
}
