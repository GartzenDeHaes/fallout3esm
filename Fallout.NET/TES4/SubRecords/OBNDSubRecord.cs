using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords
{
	public sealed class OBNDSubRecord : SubRecord
	{
		public short X1;
		public short Y1;
		public short Z1;
		public short X2;
		public short Y2;
		public short Z2;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			X1 = reader.ReadInt16();
			Y1 = reader.ReadInt16();
			Z1 = reader.ReadInt16();
			X2 = reader.ReadInt16();
			Y2 = reader.ReadInt16();
			Z2 = reader.ReadInt16();
		}
	}
}
