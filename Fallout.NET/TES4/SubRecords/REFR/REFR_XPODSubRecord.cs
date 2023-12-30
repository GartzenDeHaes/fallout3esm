using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XPODSubRecord : SubRecord
	{
		public uint Origin;
		public uint Destination;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			if (Size != 8)
			{
				Console.WriteLine("");
			}

			Origin = reader.ReadUInt32();
			Destination = reader.ReadUInt32();
		}
	}
}
