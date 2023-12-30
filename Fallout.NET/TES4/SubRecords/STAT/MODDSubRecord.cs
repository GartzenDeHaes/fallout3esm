using System;
using System.Collections.Generic;
using System.Text;

using Fallout.NET.Core;

namespace Fallout.NET.TES4.SubRecords.STAT
{
	public class MODDSubRecord : SubRecord
	{
		public MODDFaceGenModelFlags Flags;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Flags = (MODDFaceGenModelFlags)reader.ReadByte();
		}
	}

	[Flags]
	public enum MODDFaceGenModelFlags
	{
		Head = 0x01,
		Torso = 0x02,
		RightHand = 0x04,
		LeftHand = 0x08
	}
}
