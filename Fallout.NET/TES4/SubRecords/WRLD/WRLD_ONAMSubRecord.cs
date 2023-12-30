using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.WRLD
{
	public sealed class WRLD_ONAMSubRecord : SubRecord
	{
		public float WorldMapScale;
		public float CellXOffset;
		public float CellYOffset;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			WorldMapScale = reader.ReadSingle();
			CellXOffset = reader.ReadSingle();
			CellYOffset = reader.ReadSingle();
		}
	}
}
