using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Movable Static
	/// </summary>
	public sealed class MSTTRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}
	}
}