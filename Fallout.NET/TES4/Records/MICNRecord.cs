using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Menu Icon
	/// </summary>
	public sealed class MICNRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}
	}
}