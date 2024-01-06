using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	public class EYESRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			 reader.SkipBytes((int)size);
		}
	}
}