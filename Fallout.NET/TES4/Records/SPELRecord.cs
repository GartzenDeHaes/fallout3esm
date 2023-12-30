using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	public class SPELRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			 reader.ReadBytes((int)size);
		}
	}
}