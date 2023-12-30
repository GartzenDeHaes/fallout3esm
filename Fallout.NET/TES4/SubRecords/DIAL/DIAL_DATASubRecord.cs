using Fallout.NET.Core;

using System;
using System.Collections.Generic;
using System.Text;

namespace Fallout.NET.TES4.SubRecords.DIAL
{
	public sealed class DIAL_DATASubRecord : SubRecord
	{
		public DialogType Type;
		public DialogFlags Flags;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);
			Type = (DialogType)reader.ReadByte();
			Flags = (DialogFlags)reader.ReadByte();
		}
	}
}
