using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Idle Marker
	/// </summary>
	public sealed class IDLMRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.SkipBytes((int)size);
		}
	}
}