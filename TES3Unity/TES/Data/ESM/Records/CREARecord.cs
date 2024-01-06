using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum Flags
	{
		Biped = 0x0001,
		Respawn = 0x0002,
		WeaponAndShield = 0x0004,
		None = 0x0008,
		Swims = 0x0010,
		Flies = 0x0020,
		Walks = 0x0040,
		DefaultFlags = 0x0048,
		Essential = 0x0080,
		SkeletonBlood = 0x0400,
		MetalBlood = 0x0800
	}

	public sealed class CREARecord : Record, IIdRecord, IModelRecord
	{
		public string EditorId { get; private set; }
		public string Model { get; private set; }
		public string Name { get; private set; }
		public CreatureData Data { get; private set; }
		public int Flags { get; private set; }
		public string Script { get; private set; }
		public float Scale { get; set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				EditorId = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "MODL")
			{
				Model = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "FNAM")
			{
				Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "NPDT")
			{
				Data = new CreatureData
				{
					Type = reader.ReadInt32(),
					Level = reader.ReadInt32(),
					Strength = reader.ReadInt32(),
					Intelligence = reader.ReadInt32(),
					Willpower = reader.ReadInt32(),
					Agility = reader.ReadInt32(),
					Speed = reader.ReadInt32(),
					Endurance = reader.ReadInt32(),
					Personality = reader.ReadInt32(),
					Luck = reader.ReadInt32(),
					Health = reader.ReadInt32(),
					SpellPts = reader.ReadInt32(),
					Fatigue = reader.ReadInt32(),
					Soul = reader.ReadInt32(),
					Combat = reader.ReadInt32(),
					Magic = reader.ReadInt32(),
					Stealth = reader.ReadInt32(),
					AttackMin1 = reader.ReadInt32(),
					AttackMax1 = reader.ReadInt32(),
					AttackMin2 = reader.ReadInt32(),
					AttackMax2 = reader.ReadInt32(),
					AttackMin3 = reader.ReadInt32(),
					AttackMax3 = reader.ReadInt32(),
					Gold = reader.ReadInt32()
				};
			}
			else if (subRecordName == "FLAG")
			{
				Flags = (int)reader.ReadIntRecord(dataSize);
			}
			else if (subRecordName == "SCRI")
			{
				Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "XSCL")
			{
				Scale = reader.ReadSingle();
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
