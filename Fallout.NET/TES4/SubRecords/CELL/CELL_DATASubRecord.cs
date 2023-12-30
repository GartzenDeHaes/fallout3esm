using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.CELL
{
	public sealed class CELL_DATASubRecord : SubRecord
	{
		public CELL_DATAFlags Value;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Value = (CELL_DATAFlags)reader.ReadByte();
		}
	}

	[Flags]
	public enum CELL_DATAFlags
	{
		IsInteriorCell = 0x01,
		HasWater = 0x02,
		InvertFastTravelBehaviour = 0x04,
		NoLODWater = 0x08,
		Unknown = 0x10,
		PublicPlace = 0x20,
		HandChanged = 0x40,
		BehaveLikeExterior = 0x80
	}
}
