using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum AutoCalcFlagsType
	{
		Weapon = 0x00001,
		Armor = 0x00002,
		Clothing = 0x00004,
		Books = 0x00008,
		Ingrediant = 0x00010,
		Picks = 0x00020,
		Probes = 0x00040,
		Lights = 0x00080,
		Apparatus = 0x00100,
		Repair = 0x00200,
		Misc = 0x00400,
		Spells = 0x00800,
		MagicItems = 0x01000,
		Potions = 0x02000,
		Training = 0x04000,
		Spellmaking = 0x08000,
		Enchanting = 0x10000,
		RepairItem = 0x20000
	}

	public enum ClassSpecialization
	{
		Combat = 0,
		Magic = 1,
		Stealth = 2
	}

	public class CLASRecord : Record
	{
		public string Id { get; private set; }
		public string Name { get; private set; }
		public ClassData Data { get; private set; }
		public string Description { get; private set; }

		protected override void DeserializeSubRecord(ITesReader reader, GameID gameID, in AsciiId4 subRecordName, uint dataSize)
		{
			if (subRecordName == "NAME")
			{
				Id = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "FNAM")
			{
				Name = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "CLDT")
			{
				Data = new ClassData
				{
					AttributeID1 = reader.ReadInt32(),
					AttributeID2 = reader.ReadInt32(),
					Specialization = reader.ReadInt32(),
					MinorID1 = reader.ReadInt32(),
					MajorID1 = reader.ReadInt32(),
					MinorID2 = reader.ReadInt32(),
					MajorID2 = reader.ReadInt32(),
					MinorID3 = reader.ReadInt32(),
					MajorID3 = reader.ReadInt32(),
					MinorID4 = reader.ReadInt32(),
					MajorID4 = reader.ReadInt32(),
					MinorID5 = reader.ReadInt32(),
					MajorID5 = reader.ReadInt32(),
					Flags = reader.ReadInt32(),
					AutoCalcFlags = reader.ReadInt32()
				};
			}
			else if (subRecordName == "DESC")
			{
				Description = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
