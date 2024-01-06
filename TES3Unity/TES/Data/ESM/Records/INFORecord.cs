using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public class INFORecord : Record
	{
		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			ReadMissingSubRecord(reader, subRecordName, dataSize);
		}
	}
}
