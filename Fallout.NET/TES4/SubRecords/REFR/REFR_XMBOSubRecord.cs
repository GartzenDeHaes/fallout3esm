using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XMBOSubRecord : SubRecord
	{
		public float X;
		public float Y;
		public float Z;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			X = reader.ReadSingle();
			Y = reader.ReadSingle();
			Z = reader.ReadSingle();
		}
	}
}
