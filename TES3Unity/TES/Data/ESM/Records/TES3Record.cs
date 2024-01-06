using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum TESHeaderType
	{
		ESP = 0, ESM = 1, ESS = 2
	}

	public sealed class TES3Record : Record
	{
		public float Version;
		public TESHeaderType Type;
		public string Company;
		public string Description;
		public uint RecordCount;
		public string Master;
		public long PreviousMasterSize;

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "HEDR")
			{
				Version = reader.ReadSingle();
				Type = (TESHeaderType)reader.ReadUInt32();
				Company = reader.ReadASCIIString(32);
				Description = reader.ReadASCIIString(256);
				RecordCount = reader.ReadUInt32();
			}
			else if (subRecordName == "MAST")
			{
				Master = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "DATA")
			{
				PreviousMasterSize = reader.ReadInt64();
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
