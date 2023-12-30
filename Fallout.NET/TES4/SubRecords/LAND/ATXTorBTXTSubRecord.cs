using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.LAND
{
	public sealed class ATXTorBTXTSubRecord : SubRecord
	{
		public uint Texture;
		public ATXTorBTXTQuadrants Quadrant;
		public byte Unused;
		public short Layer;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Texture = reader.ReadUInt32();
			Quadrant = (ATXTorBTXTQuadrants)reader.ReadByte();
			Unused = reader.ReadByte();
			Layer = reader.ReadInt16();
		}
	}

	public enum ATXTorBTXTQuadrants
	{
		BottomLeft = 0,
		BottomRight,
		TopLeft,
		TopRight
	}
}
