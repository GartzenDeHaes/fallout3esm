using Fallout.NET.Core;

namespace Fallout.NET.TES4.Records
{
	public class SKILRecord : Record
	{
		protected override void ExtractSubRecords(BetterReader reader, GameID gameID, uint size)
		{
			reader.ReadBytes((int)size);
		}
	}

	public enum SkillType
	{
		None = -1,
		Barter = 0,
		BigGuns = 1,
		EnergyWeapons = 2,
		Explosives = 3,
		Lockpick = 4,
		Medicine = 5,
		MeleeWeapons = 6,
		Repair = 7,
		Science = 8,
		SmallGuns = 9,
		Sneak = 10,
		Speech = 11,
		Throwing = 12, //(unused)
		Unarmed = 13,
	}
}