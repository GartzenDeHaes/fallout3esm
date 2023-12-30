using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.LAND
{
	public sealed class VTXTSubRecord : SubRecord
	{
		public List<VTXTStruct> Records;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Records = new List<VTXTStruct>();

			var currentPosition = 0;
			while (currentPosition < Size)
			{
				Records.Add(new VTXTStruct
				{
					Position = reader.ReadUInt16(),
					Unused = reader.ReadBytes(2),
					Opacity = reader.ReadSingle()
				});
				currentPosition += 8;
			}
		}
	}

	public struct VTXTStruct
	{
		public ushort Position;
		public byte[] Unused;
		public float Opacity;
	}
}
