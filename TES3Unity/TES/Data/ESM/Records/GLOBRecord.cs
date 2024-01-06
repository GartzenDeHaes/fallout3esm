using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class GLOBRecord : Record, IIdRecord
	{
		public string EditorId { get; private set; }
		public string Name { get; private set; }
		public float FloatValue { get; private set; }

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
			else if (subRecordName == "FLTV")
			{
				FloatValue = reader.ReadSingle();
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
