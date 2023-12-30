using Fallout.NET.Core;

using System;
using System.Collections.Generic;
using System.Text;

namespace Fallout.NET.TES4.SubRecords.INFO
{
	public sealed class INFO_TRDTSubrecord : SubRecord
	{
		public DialogResponseEmotionType EmotionType;
		public int EmotionValue;
		public byte[] Unused1;
		public byte ResponseNumber;
		public byte[] Unused2;
		public int SoundFormId;
		public DialogResponseFlags Flags;
		public byte[] Unused3;

		public override void Deserialize(BetterReader reader, string name)
		{
			base.Deserialize(reader, name);

			if (Size < 16)
			{
				UnityEngine.Debug.Log($"{Size}");
			}

			EmotionType = (DialogResponseEmotionType)reader.ReadUInt32();
			EmotionValue = reader.ReadInt32();
			Unused1 = reader.ReadBytes(4);
			ResponseNumber = reader.ReadByte();
			Unused2 = reader.ReadBytes(3);

			if (Size > 16)
			{
				SoundFormId = reader.ReadInt32();
			}

			if (Size > 20)
			{
				Flags = (DialogResponseFlags)reader.ReadByte();
				Unused3 = reader.ReadBytes(3);
			}
		}
	}
}
