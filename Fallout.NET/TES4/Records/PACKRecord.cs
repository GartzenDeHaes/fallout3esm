using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// AI Package
	/// https://tes5edit.github.io/fopdoc/Fallout3/Records/PACK.html
	/// </summary>
	public class PACKRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			 reader.SkipBytes((int)size);
		}
	}
}