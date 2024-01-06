using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class SCPTRecord : Record
	{
		public ScriptHeader Metadata { get; private set; }
		public string LocalVariables { get; private set; }
		public byte[] Binary { get; private set; }
		public string Text { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "SCHD")
			{
				Metadata = new ScriptHeader
				{
					Name = reader.ReadStringFromChar(32),
					NumShorts = reader.ReadUInt32(),
					NumLongs = reader.ReadUInt32(),
					NumFloats = reader.ReadUInt32(),
					ScriptDataSize = reader.ReadUInt32(),
					LocalVarSize = reader.ReadUInt32()
				};
			}
			else if (subRecordName == "SCVR")
			{
				LocalVariables = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "SCDT")
			{
				Binary = reader.ReadBytes((int)dataSize);
			}
			else if (subRecordName == "SCTX")
			{
				Text = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
		}
	}
}
