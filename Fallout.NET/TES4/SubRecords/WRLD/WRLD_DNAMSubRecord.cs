using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.WRLD
{
	public sealed class WRLD_DNAMSubRecord : SubRecord
	{
		public float DefaultLandHeight;
		public float DefaultWaterHeight;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			DefaultLandHeight = reader.ReadSingle();
			DefaultWaterHeight = reader.ReadSingle();
		}
	}
}
