using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Placed Grenade
	/// </summary>
	public sealed class PGRERecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.SkipBytes((int)size);
		}
	}
}