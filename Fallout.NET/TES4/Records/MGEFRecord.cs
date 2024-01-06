using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Magic Effect
	/// https://tes5edit.github.io/fopdoc/Fallout3/Records/MGEF.html
	/// </summary>
	public class MGEFRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			 reader.SkipBytes((int)size);
		}
	}
}