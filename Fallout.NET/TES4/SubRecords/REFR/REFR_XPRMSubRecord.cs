using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public sealed class REFR_XPRMSubRecord : SubRecord
	{
		public float XBound;
		public float YBound;
		public float ZBound;
		public float Red;
		public float Green;
		public float Blue;
		public byte[] Unknown;
		public REFR_XPRMTypes Type;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			XBound = reader.ReadSingle();
			YBound = reader.ReadSingle();
			ZBound = reader.ReadSingle();
			Red = reader.ReadSingle();
			Green = reader.ReadSingle();
			Blue = reader.ReadSingle();
			Unknown = reader.ReadBytes(4);
			Type = (REFR_XPRMTypes)reader.ReadUInt32();
		}
	}

	public enum REFR_XPRMTypes
	{
		None = 0,
		Box,
		Sphere,
		PortalBox
	}
}
