using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Load Screen
	/// https://tes5edit.github.io/fopdoc/Fallout3/Records/LSCR.html
	/// </summary>
	public class LSCRRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			 reader.SkipBytes((int)size);
		}
	}
}