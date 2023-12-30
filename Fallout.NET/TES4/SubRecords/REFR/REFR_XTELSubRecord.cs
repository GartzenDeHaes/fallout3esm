using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XTELSubRecord : SubRecord
	{
		public uint Door;
		public float X;
		public float Y;
		public float Z;
		public float rX;
		public float rY;
		public float rZ;
		public REFR_XTELFlags Flags;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Door = reader.ReadUInt32();
			X = reader.ReadSingle();
			Y = reader.ReadSingle();
			Z = reader.ReadSingle();
			rX = reader.ReadSingle();
			rY = reader.ReadSingle();
			rZ = reader.ReadSingle();
			Flags = (REFR_XTELFlags)reader.ReadUInt32();
		}
	}

	[Flags]
	public enum REFR_XTELFlags
	{
		NoAlarm = 0x01
	}
}
