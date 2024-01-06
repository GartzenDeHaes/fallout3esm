
using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class SOUNRecord : Record, IIdRecord
	{
		public string EditorId { get; private set; }
		public string Name;
		public byte Volume;
		public byte MinRange;
		public byte MaxRange;

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				EditorId = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "FNAM")
			{
				Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "DATA")
			{
				Volume = reader.ReadByte();
				MinRange = reader.ReadByte();
				MaxRange = reader.ReadByte();
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
