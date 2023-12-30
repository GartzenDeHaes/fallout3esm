using Fallout.NET.Core;
using Fallout.NET.TES4.SubRecords.DIAL;

using System;
using System.Collections.Generic;
using System.Text;

namespace Fallout.NET.TES4.SubRecords.INFO
{
	public sealed class INFO_DATASubRecord : SubRecord
	{
		public DialogType Type;
		public NextSpeaker NextSpeaker;
		public INFOFlags Flags;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			Type = (DialogType)reader.ReadByte();
			NextSpeaker = (NextSpeaker)reader.ReadByte();
			if (Size > 3)
			{
				Flags = (INFOFlags)BitConverter.ToUInt16(reader.ReadBytes(2), 0);
			}
			else
			{
				Flags = (INFOFlags)reader.ReadByte();
			}
		}
	}
}
