using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.REFR
{
	public class REFR_XNDPSubRecord : SubRecord
	{
		public uint NavigationMesh;
		public byte[] Unknown;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			NavigationMesh = reader.ReadUInt32();
			Unknown = reader.ReadBytes(4);
		}
	}
}
