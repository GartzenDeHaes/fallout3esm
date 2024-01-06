using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	/// <summary>
	/// Land texture
	/// </summary>
	public sealed class LTEXRecord : Record, IIdRecord
	{
		public string EditorId { get; private set; }
		public long Index { get; private set; }
		public string Texture { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				EditorId = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "INTV")
			{
				Index = reader.ReadIntRecord(dataSize);
			}
			else if (subRecordName == "DATA")
			{
				Texture = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
