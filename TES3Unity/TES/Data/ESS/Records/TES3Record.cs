using Fallout.NET;

using Portland;

using TES3Unity.ESM;

namespace TES3Unity.ESS.Records
{
	public struct GameDataInfo
	{
		public float Health;
		public float MaxHealth;
		public float Time;
		public float Month;
		public float Day;
		public float Year;
		public string Cell;
		public float DayPassed;
		public string CharacterName;
	}

	public sealed class TES3Record : Record
	{
		public GameDataInfo GameData { get; private set; }
		public string CellName => Convert.RemoveNullChar(GameData.Cell);

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "GMDT")
			{
				GameData = new GameDataInfo
				{
					Health = reader.ReadSingle(),
					MaxHealth = reader.ReadSingle(),
					Time = reader.ReadSingle(),
					Month = reader.ReadSingle(),
					Day = reader.ReadSingle(),
					Year = reader.ReadSingle(),
					Cell = reader.ReadPossiblyNullTerminatedASCIIString(64),
					DayPassed = reader.ReadSingle(),
					CharacterName = reader.ReadPossiblyNullTerminatedASCIIString(32)
				};
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
