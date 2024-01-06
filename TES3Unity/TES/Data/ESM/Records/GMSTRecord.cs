using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class GMSTRecord : Record, IIdRecord
	{
		public string EditorId { get; private set; }
		public string StringValue { get; private set; }
		public int IntValue { get; private set; }
		public float FloatValue { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				EditorId = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "STRV")
			{
				StringValue = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "INTV")
			{
				IntValue = (int)reader.ReadIntRecord(dataSize);
			}
			else if (subRecordName == "FLTV")
			{
				FloatValue = reader.ReadSingle();
			}
		}
	}
}
