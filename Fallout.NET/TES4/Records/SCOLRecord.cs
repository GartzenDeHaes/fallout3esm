using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Static Collection
	/// https://tes5edit.github.io/fopdoc/Fallout3/Records/SCOL.html
	/// </summary>
	public sealed class SCOLRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.SkipBytes((int)size);
		}
	}
}