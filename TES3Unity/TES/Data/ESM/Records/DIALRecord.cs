﻿using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum DialogueTopic
	{
		RegularTopic = 0,
		Voice = 1,
		Greeting = 2,
		Persuasion = 3,
		Journal = 4
	}

	public sealed class DIALRecord : Record
	{
		public string Id { get; private set; }
		public DialogueTopic Topic { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "DATA")
			{
				Topic = (DialogueTopic)reader.ReadByte();
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
