using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Note
	/// </summary>
	public sealed class NOTERecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}
	}
}