using System.Collections.Generic;

using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public enum ArmorType
	{
		Helmet = 0,
		Cuirass,
		LPauldron,
		RPauldron,
		Greaves,
		Boots,
		LGauntlet,
		RGauntlet,
		Shield,
		LBracer,
		RBracer,
	}

	public enum BodyPartIndex
	{
		Head = 0,
		Hair,
		Neck,
		Cuirass,
		Groin,
		Skirt,
		RightHand,
		LeftHand,
		RightWrist,
		LeftWrist,
		Shield,
		RightForearm,
		LeftForearm,
		RightUpperArm,
		LeftUpperArm,
		RightFoot,
		LeftFoot,
		RightAnkle,
		LeftAnkle,
		RightKnee,
		LeftKnee,
		RightUpperLeg,
		LeftUpperLeg,
		RightPauldron,
		LeftPauldron,
		Weapon,
		Tail
	}

	public class ARMORecord : Record, IIdRecord, IModelRecord
	{
		public string EditorId { get; private set; }
		public string Model { get; private set; }
		public string Name { get; private set; }
		public ArmorData Data { get; private set; }
		public string Icon { get; private set; }
		public List<ArmorBodyPartGroup> BodyPartGroup { get; private set; }
		public string Script { get; private set; }
		public string Enchantment { get; private set; }

		public ARMORecord()
		{
			BodyPartGroup = new List<ArmorBodyPartGroup>();
		}

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
			else if (subRecordName == "AODT")
			{
				Data = new ArmorData
				{
					Type = (ArmorType)reader.ReadInt32(),
					Weight = reader.ReadSingle(),
					Value = reader.ReadInt32(),
					Health = reader.ReadInt32(),
					EnchantPts = reader.ReadInt32(),
					Armour = reader.ReadInt32()
				};
			}
			else if (subRecordName == "ITEX")
			{
				Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "INDX")
			{
				BodyPartGroup.Add(new ArmorBodyPartGroup
				{
					Index = (BodyPartIndex)reader.ReadIntRecord(dataSize)
				});
			}
			else if (subRecordName == "BNAM")
			{
				var last = BodyPartGroup.Count - 1;
				var partName = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
				var part = BodyPartGroup[last];

				BodyPartGroup[last] = new ArmorBodyPartGroup
				{
					FemalePartName = part.FemalePartName,
					Index = part.Index,
					MalePartName = partName
				};
			}
			else if (subRecordName == "CNAM")
			{
				var last = BodyPartGroup.Count - 1;
				var partName = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
				var part = BodyPartGroup[last];

				BodyPartGroup[last] = new ArmorBodyPartGroup
				{
					FemalePartName = partName,
					Index = part.Index,
					MalePartName = part.MalePartName
				};
			}
			else if (subRecordName == "SCRI")
			{
				Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "ENAM")
			{
				Enchantment = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
