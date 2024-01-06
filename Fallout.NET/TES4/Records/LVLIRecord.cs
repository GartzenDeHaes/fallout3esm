using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	/// <summary>
	/// Leveled Item (changes based on player level)
	/// https://en.uesp.net/wiki/Skyrim_Mod:Mod_File_Format/LVLI
	/// </summary>
	public class LVLIRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			 reader.SkipBytes((int)size);
		}
	}
}