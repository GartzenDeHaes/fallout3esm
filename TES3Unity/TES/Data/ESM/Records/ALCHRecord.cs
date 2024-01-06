using Fallout.NET;

using Portland;

namespace TES3Unity.ESM.Records
{
	public sealed class ALCHRecord : Record, IIdRecord, IModelRecord
	{
		public string EditorId { get; private set; }
		public string Model { get; private set; }
		public string Name { get; private set; }
		public AlchemyData Data { get; private set; }
		public SingleEnchantData Enchantment { get; private set; }
		public string Icon { get; private set; }
		public string Script { get; private set; }

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
			else if (subRecordName == "ALDT")
			{
				Data = new AlchemyData
				{
					Weight = reader.ReadSingle(),
					Value = reader.ReadInt32(),
					AutoCalc = reader.ReadInt32()
				};
			}
			else if (subRecordName == "ENAM")
			{
				Enchantment = new SingleEnchantData
				{
					EffectID = reader.ReadInt16(),
					SkillID = reader.ReadByte(),
					AttributeID = reader.ReadByte(),
					RangeType = (EnchantRangeType)reader.ReadInt32(),
					Area = reader.ReadInt32(),
					Duration = reader.ReadInt32(),
					MagMin = reader.ReadInt32(),
					MagMax = reader.ReadInt32()
				};
			}
			else if (subRecordName == "TEXT")
			{
				Icon = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else if (subRecordName == "SCRI")
			{
				Script = reader.ReadPossiblyNullTerminatedASCIIString((int)dataSize);
			}
			else
			{
				ReadMissingSubRecord(reader, subRecordName, dataSize);
			}
		}
	}
}
