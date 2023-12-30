using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.WRLD
{
	public sealed class WRLD_MNAMSubRecord : SubRecord
	{
		public int UseableXSize;
		public int UseableYSize;
		public short NWCellXCoordinate;
		public short NWCellYCoordinate;
		public short SECellXCoordinate;
		public short SECellYCoordinate;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			UseableXSize = reader.ReadInt32();
			UseableYSize = reader.ReadInt32();
			NWCellXCoordinate = reader.ReadInt16();
			NWCellYCoordinate = reader.ReadInt16();
			SECellXCoordinate = reader.ReadInt16();
			SECellYCoordinate = reader.ReadInt16();
		}
	}
}
