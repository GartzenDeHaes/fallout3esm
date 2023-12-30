using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Talking Activator
	/// </summary>
	public sealed class TACTRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}
	}
}