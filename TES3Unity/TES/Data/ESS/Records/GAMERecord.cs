using Fallout.NET;

using Portland;

using TES3Unity.ESM;

namespace TES3Unity.ESS.Records
{
	public sealed class GAMERecord : Record
	{
		public string CellName { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "GMDT")
			{
				CellName = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
